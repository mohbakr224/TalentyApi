# Talentry AI System — Documentation

Full technical reference for the Talentry AI backend. Every public function is typed, validated against a Pydantic schema, and documented in-code with a `DATA CONTRACT` docstring (inputs, outputs, error cases, and an example call/response) — this document summarizes and cross-references those contracts, organized by architectural layer rather than by source location.

## Contents

1. [Architecture overview](#1-architecture-overview)
2. [Foundation layer: LLM router & schema-safe invocation](#2-foundation-layer-llm-router--schema-safe-invocation)
3. [Data models](#3-data-models)
4. [Candidate Profile: extraction & evaluation](#4-candidate-profile-extraction--evaluation)
5. [Skill normalization](#5-skill-normalization)
6. [Resume / ATS scoring & CV↔JD matching](#6-resume--ats-scoring--cvjd-matching)
7. [Portfolio analysis](#7-portfolio-analysis)
8. [Job description parsing](#8-job-description-parsing)
9. [AI interview engine](#9-ai-interview-engine)
10. [Skill Gap Engine](#10-skill-gap-engine)
11. [Career Readiness Orchestrator](#11-career-readiness-orchestrator)
12. [Known limitations / TODOs](#12-known-limitations--todos)

---

## 1. Architecture overview

Talentry AI is a set of independent, composable evaluators built on one shared foundation, rather than a single monolithic pipeline. Every feature can be called on its own; several are also designed to be fed each other's output for progressively richer analysis.

**Layers, bottom to top:**

- **Foundation** — a single routed LLM client (`LLMRouter`) and a schema-safe invocation helper (`invoke_with_schema`) that every feature calls through. Nothing talks to Groq directly.
- **Data contracts** — one Pydantic model per feature's output (plus shared sub-models), so every LLM call is instructed, via `format_instructions`, to return JSON matching an exact schema, and every response is validated before it reaches a caller.
- **Extraction features** — turn unstructured input into structured data: `ExtractCandidateProfile` (raw text → profile), `ParseJobDescription`/`IntakeJobDescription` (raw posting → structured JD), `Resume_analysis` (PDF/DOCX → plain text), `scrape_to_context` (portfolio URL → plain text).
- **Evaluation features** — score structured or extracted content: `ProfileEvaluation`, `ResumeEvaluation`, `ComputeJDMatch`, `PortfolioEvaluation`, and the interview engine (`CreateInterview` / `SubmitAnswer` / `EndInterview`).
- **Cross-cutting analysis** — `AnalyzeSkillGap` and `EvaluateCareerReadiness` sit above the evaluators and consume their outputs rather than re-deriving anything themselves; `EvaluateCareerReadiness` in particular never re-runs Resume/Portfolio/Interview analysis — it only aggregates.

**Shared conventions across every feature:**

- **Structured I/O.** Every LLM-backed feature pairs a `PromptTemplate` with a `PydanticOutputParser`, so the schema is baked into the prompt itself.
- **Retry-on-invalid-parse.** A failed schema validation re-prompts the LLM with the validation error and its previous (bad) output attached, up to `max_retries` extra attempts, before raising `RuntimeError`. This is centralized in `invoke_with_schema` for the newer features, and duplicated inline in a few of the earlier ones (`ExtractCandidateProfile`, `ResumeEvaluation`, `PortfolioEvaluation`, `ParseJobDescription`) that predate that helper.
- **Model routing with fallback.** Every LLM call goes through one `llm` object (`LLMRouter`), which tries a small/fast primary model and transparently falls back to a larger model on any exception.
- **JSON-string outputs.** Every public function returns a JSON string (`model_dump_json(...)` / `json.dumps(...)`), not a Python dict — callers `json.loads()` the result.

**Typical data flow:**

```
raw CV text ──► ExtractCandidateProfile ──► CandidateProfile ──┬─► ProfileEvaluation
                                                                 ├─► ResumeEvaluation
raw JD text ──► IntakeJobDescription ──► JDAnalysisResult ──────┤    (also used by ComputeJDMatch)
                                                                 ├─► PortfolioEvaluation (URL, not JD)
                                                                 ├─► CreateInterview (as context)
                                                                 ├─► AnalyzeSkillGap
                                                                 └─► EvaluateCareerReadiness
                                                                      (aggregates all of the above)
```

## 2. Foundation layer: LLM router & schema-safe invocation

- `llm_primary` = `ChatGroq(model='openai/gpt-oss-20b', temperature=0, max_tokens=8192)`
- `llm_fallback` = `ChatGroq(model='openai/gpt-oss-120b', temperature=0, max_tokens=8192)`
- **`LLMRouter.invoke(prompt_str)`** — tries `primary`, logs and falls back to `fallback` on any exception. Instantiated once as `llm = LLMRouter(llm_primary, llm_fallback)` and reused by every feature — no feature instantiates its own model.
- **`invoke_with_schema(prompt_str, parser, max_retries=2)`** — calls `llm.invoke`, parses the response against `parser`, and on failure re-prompts with the validation error plus the bad output appended, up to `max_retries` extra times. Raises `RuntimeError` after `max_retries + 1` total failed attempts. This is the shared retry mechanism used by `ProfileEvaluation` and the entire interview / skill-gap / career-readiness stack.

Credentials (a `GROQ_API_KEY`) are expected in the environment; `ChatGroq` reads it automatically. In the reference notebook this is loaded from Colab's secret manager — set it directly as an environment variable in any other runtime.

## 3. Data models

All Pydantic schemas are defined together as one shared contract layer, grouped here by feature area:

**Candidate profile**
- `Experience`, `Education`, `Project`, `Portfolio` — sub-entries of a profile.
- `CandidateProfile` — the full structured candidate record: identity, professional summary, role targeting, skills, experience, education, projects/portfolio links, certifications, availability. Output of `ExtractCandidateProfile`.
- `ProfileEvaluationResult` — output of `ProfileEvaluation`: `completenessScore`, `qualityScore`, `missingFields`, `weakFields`, `strengths`, `recommendations`, `summary`.

**Resume / CV**
- `CVSections` — per-section feedback (experience, education, skills, projects, summary, achievements, keywords).
- `CVAnalysisResult` — output of `ResumeEvaluation`: `score`, `atsScore` (LLM semantic judgment), `atsTfidfScore` (deterministic), `grammarScore`, `formattingScore`, `sections`, `strengths`, `weaknesses`, `itemsToRemove`, `missingSkills`, `recommendations`.

**Portfolio**
- `PortfolioSections` — design, content, projects, professionalism, mobileExperience, recruiterExperience, roleClarity.
- `ProjectFeedback` — one issue on one named project.
- `PortfolioAnalysisResult` — output of `PortfolioEvaluation`: `score`, `sections`, `strengths`, `weaknesses`, `missingElements`, `recommendations`, `projectFeedback`.

**Job description**
- `JDAnalysisResult` — output of `ParseJobDescription`/`IntakeJobDescription`: title, company, seniority, employment/work mode, location, salary, required/preferred/soft skills, responsibilities, qualifications, keywords.
- `JDMatchResult` — output of `ComputeJDMatch`: `matchPercentage`, `matchedSkills`, `missingSkills`, `extraSkills`.

**Interview engine**
- Enums: `InterviewType` (HR / BEHAVIORAL / TECHNICAL / SITUATIONAL — MVP scope), `CandidateLevel` (JUNIOR/MID/SENIOR), `Difficulty` (EASY/MEDIUM/HARD), `InterviewLanguage` (EN/AR, with a `.full_name` property), `InterviewMode` (TEXT/VOICE/VIDEO — voice/video are expected to already be transcribed before reaching `SubmitAnswer`), `InterviewStatus` (CREATED/IN_PROGRESS/COMPLETED/ABANDONED).
- `INTERVIEW_LIMITS` / `QUESTION_COUNT_GUIDE` — per-type min/max main questions, max follow-ups per main question, and max total turns; these are the *hard* limits the code enforces regardless of what the LLM proposes.
- `ContextProviders` — a bundle of loader callables (`load_candidate`, `load_job`, `load_resume`, `load_portfolio`, `load_skills`); each defaults to a stub (`_stub_loader`) that reports `status: "not_available"` until the real data layer is wired in. `DEFAULT_PROVIDERS` is the all-stub default.
- `QuestionFeedback`, `InterviewTurn`, `InitialQuestion`, `NextTurnDecision`, `InterviewEvaluation`, `InterviewReport`, `InterviewSession` — per-answer feedback, one Q/A turn, the opening-question shape, the next-turn decision shape, the holistic end-of-interview evaluation, the final report, and the full session state respectively.

**Skill gap**
- `SkillPriority` — one skill with status (missing/weak) and priority.
- `SkillGapAnalysisResult` — output of `AnalyzeSkillGap`.

**Career readiness**
- `DimensionWeight` — one LLM-proposed `{dimension, weight, rationale}` triple.
- `ReadinessDimension` — one scored, weighted dimension in the final report.
- `CriticalGap`, `NextAction`, `CareerReadinessNarrative`, `CareerReadinessResult` — the narrative pieces and the final aggregated output of `EvaluateCareerReadiness`.

> Note: an early draft of the interview-engine *functions* (`invoke_with_schema`, `CreateInterview`, `SubmitAnswer`, `EndInterview`) exists alongside the models, predating the polished versions described in [§9](#9-ai-interview-engine). Only the polished versions are what the system actually runs with; the models/enums/prompts they share are the single source of truth regardless.

## 4. Candidate Profile: extraction & evaluation

### `ExtractCandidateProfile(text: str, max_retries: int = 2) -> dict`
Extracts a structured `CandidateProfile` from raw text (CV content, onboarding form, or free-form bio). Unmentioned fields are left `null`.
- **Errors**: `RuntimeError` after `max_retries + 1` failed parse attempts.

### `ProfileEvaluation(candidate: dict, max_retries: int = 2) -> dict`
Scores a `CandidateProfile` dict for completeness and quality ahead of it being shown to recruiters. Combines a deterministic reference (`_compute_completeness_ref`: % of `PROFILE_IMPORTANT_FIELDS` filled) with the LLM's own judgment of what matters for that candidate — the deterministic number is a starting point, not ground truth.
- **Output**: `ProfileEvaluationResult`.
- **Errors**: `RuntimeError` after `max_retries + 1` failed parse attempts.

## 5. Skill normalization

A standalone utility layer used wherever two skill lists need to be compared fairly. Canonicalizes every spelling/alias of a skill ("JS", "ReactJS", "k8s", "ML"...) to one lowercase form, and matches skills in free text on word boundaries (so "js" doesn't match inside "json", "go" inside "google", "ml" inside "html").

Public API:
- `normalize_skill(skill) -> str` — canonical form of one skill (or its cleaned form if not a known alias).
- `normalize_skills(skills: list) -> list` — canonicalizes + dedupes a list, preserving first-seen order.
- `skill_in_text(skill, text) -> bool` — whether a skill (any alias) appears in `text` as a whole word.

`SKILL_ALIASES` is the canonical-name → alias-list map (programming languages, ML/AI terms, frameworks, cloud platforms, soft skills, etc.). `_AMBIGUOUS_BARE = {"go", "r", "c"}` excludes those bare single-letter/short forms from being matched as standalone words in text (too many false positives), while still recognizing them when a skill list explicitly states them. Consumed by `ComputeJDMatch` and `AnalyzeSkillGap`.

## 6. Resume / ATS scoring & CV↔JD matching

### `Resume_analysis(file_path) -> str`
Extracts raw text from a `.pdf` (`pypdf`) or `.docx` (`docx2txt`) resume file. Raises `ValueError` for any other extension.

### `compute_tfidf_ats_score(resume_text, description) -> float`
Deterministic TF-IDF cosine-similarity score (0–100) between resume text and a job description string. Returns `0.0` if `description` is empty.

### `ComputeJDMatch(resume_text: str, parsed_jd: dict) -> dict`
Deterministic CV↔JD match: TF-IDF similarity averaged with normalized-skill overlap. No LLM call involved.
- **Output**: `JDMatchResult` — `matchPercentage`, `matchedSkills`, `missingSkills`, `extraSkills` (currently always `[]`).
- **Errors**: none — returns 0% scores if the JD has no listed skills.

### `ResumeEvaluation(file_path: str, description: str | dict | None = None, max_retries: int = 2) -> dict`
Extracts resume text, optionally parses/accepts a job description, computes the TF-IDF reference score, then asks the LLM for a full `CVAnalysisResult` (score, ATS, grammar, formatting, section feedback, strengths/weaknesses, items to remove, missing skills, recommendations). If no JD is given, evaluates general ATS compatibility and quality instead of a JD match.
- **Errors**: `ValueError` for unsupported file types; `RuntimeError` after `max_retries + 1` failed parse attempts.

## 7. Portfolio analysis

An adapter pattern routes a portfolio URL to the right extraction strategy:

- `GitHubAdapter` — matches `github.com` URLs; pulls profile + non-fork repo metadata via the GitHub REST API (no HTML scraping).
- `UnsupportedPlatformAdapter` — matches `behance.net` / `dribbble.com`; returns `status: "UNSUPPORTED_SOURCE"` since no reliable automated extraction exists for them.
- `GenericWebsiteAdapter` — fallback for personal sites / Vercel deploys / anything else; breadth-first crawls same-domain pages (`requests` + `BeautifulSoup`, up to `max_pages`, 0.5s delay between requests), strips boilerplate tags, and dedupes chunks.

`PORTFOLIO_ADAPTERS = [GitHubAdapter(), UnsupportedPlatformAdapter(), GenericWebsiteAdapter()]` — checked in order, so `GenericWebsiteAdapter` (which `supports()` everything) must stay last.

### `scrape_to_context(url, max_pages=50) -> dict`
Routes `url` to the first matching adapter and returns `{status, platform, url, content}`. `status` is `"OK"`, `"PARTIAL"` (crawl hit `max_pages` or found nothing), or `"UNSUPPORTED_SOURCE"`.

### `PortfolioEvaluation(url: str, target_role: str | None = None, max_retries: int = 2) -> dict`
Scrapes the portfolio, then asks the LLM to evaluate design/UX, content, project depth, professionalism, mobile experience, and recruiter experience for the given `target_role`.
- **Output**: `PortfolioAnalysisResult` with a `status` field added (`OK`/`PARTIAL`), or `{"status": "UNSUPPORTED_SOURCE", "platform", "url", "score": null, "message"}` for unsupported platforms.
- **Errors**: `RuntimeError` after `max_retries + 1` failed LLM parse attempts.

## 8. Job description parsing

### `ParseJobDescription(jd_text: str, max_retries: int = 2) -> dict`
Extracts structured fields from a raw job posting: title, company, seniority, employment/work mode, location, salary, required/preferred/soft skills, responsibilities (max 8, verb-first), qualifications, ATS keywords (including both abbreviations and full forms, e.g. "ML" and "machine learning"). Unmentioned fields are left empty/null rather than invented.
- **Output**: `JDAnalysisResult`.
- **Errors**: `RuntimeError` after `max_retries + 1` failed parse attempts.

### `IntakeJobDescription(jd_text: str) -> dict`
Thin wrapper around `ParseJobDescription` — the intended single entry point to call once per JD; downstream features (`ResumeEvaluation`, `ComputeJDMatch`, `AnalyzeSkillGap`, `EvaluateCareerReadiness`) accept its output dict directly instead of re-parsing.

## 9. AI interview engine

The most stateful part of the system: a multi-turn loop built around an `InterviewSession` object that the caller passes back and forth between calls.

### `CreateInterview(candidate_id, interview_type, level, difficulty, language=EN, job_id=None, target_role=None, mode=TEXT, providers=None) -> dict`
Loads candidate/job/resume/portfolio/skills context (via `providers`, defaulting to stub loaders), creates an `InterviewSession`, and generates the opening question via the LLM.
- **Output**: `{"session": InterviewSession dict, "question": str}`.

### `SubmitAnswer(session: dict, answer_text: str) -> dict`
Records the answer on the current turn, scores it with `question_feedback_prompt` (`QuestionFeedback`), then determines the next action.
- **Deterministic guardrails**: `_turn_state` computes what's actually allowed right now from `INTERVIEW_LIMITS` (can follow up? can ask a new main question? can end?) — the LLM's `NextTurnDecision` is only accepted if `_decision_allowed` confirms it matches the allowed state; otherwise `_decide_next_turn` re-prompts (up to `max_attempts`) and finally forces `should_end=True` if the LLM keeps proposing disallowed actions. This keeps question counts and follow-up depth within `INTERVIEW_LIMITS` regardless of what the LLM decides — the LLM proposes, the code disposes.
- **Output**: `{"session", "question", "is_followup", "should_end", "last_feedback"}`.
- **Errors**: `ValueError` if the session isn't `IN_PROGRESS`, or has no open question to answer.

### `EndInterview(session: dict) -> dict`
Runs a holistic evaluation over the full transcript (`InterviewEvaluation`) and builds the final `InterviewReport`.
- **Output**: `{"session", "evaluation", "report"}`.
- **Errors**: `ValueError` if there are no answered turns to evaluate.

## 10. Skill Gap Engine

Independent of `ResumeEvaluation`: that feature scores the skills already present in a CV, while this engine asks "what does this candidate still need for *this* role/JD," using every available signal.

### `AnalyzeSkillGap(candidate, target_role=None, job_description=None, job_description_text=None, resume_evaluation=None, portfolio_evaluation=None, interview_result=None, max_retries=2) -> dict`
Computes deterministic reference sets (current skills from the candidate profile, required skills from the JD, matched/missing via `normalize_skills`), builds a context block from every optional evaluator result passed in, and asks the LLM to produce the final gap analysis, priority list, and readiness score. Output skill lists are re-normalized and deduped after the LLM call so casing/aliases stay consistent.
- **Output**: `SkillGapAnalysisResult` — `target_role`, `currentSkills`, `requiredSkills`, `matchedSkills`, `missingSkills`, `weakSkills`, `priority` (list of `SkillPriority`), `readinessScore`, `summary`, `recommendations`.
- **Errors**: `ValueError` if none of `target_role`, `job_description`, or `job_description_text` is given; `RuntimeError` after `max_retries + 1` failed parse attempts.

## 11. Career Readiness Orchestrator

The top-level aggregator, and the clearest illustration of the "compose, don't recompute" principle in this architecture: it never re-runs Resume/Portfolio/Interview analysis itself — it only consumes their outputs.

### `EvaluateCareerReadiness(candidate, target_role=None, job_description=None, job_description_text=None, resume_evaluation=None, portfolio_evaluation=None, profile_evaluation=None, skill_gap_result=None, interview_result=None, weight_overrides=None, max_retries=2) -> dict`
- **Dynamic dimension set**: dimensions considered are exactly whichever evaluator outputs were actually passed in (`_collect_dimension_scores` returns `None` for anything not provided, and those are dropped) — there is no fixed dimension list or fixed role→weights table.
- **Readiness mode**: `"job_specific"` if a job description is available (directly or derived from `job_description_text`), else `"general"`.
- **Weighting** (`_apply_dimension_weights`): the LLM proposes a rationale + relative weight per dimension (`CareerReadinessNarrative.dimensionWeights`); `weight_overrides` (a `{dimension_key: weight}` map, each 0–1, pins summing to ≤ 1.0) can pin exact shares for this call only, with the LLM's weights splitting the remaining share across the rest. Invalid/duplicate/unknown override keys are validated by `_validate_weight_overrides` and raise `ValueError` on invalid input (e.g. pins summing above 1.0).
- **Output**: `CareerReadinessResult` — `target_role`, `readiness_mode`, `overallReadinessScore`, `readinessLevel` (`_readiness_level`: highly_ready ≥85, ready ≥70, developing ≥50, emerging ≥30, else not_ready), `dimensions` (only those with evidence), `dataCompleteness`, `criticalGaps`, `strengths`, `recommendations`, `nextActions`, `summary`.
- **Errors**: `ValueError` if no evaluator result has usable data, or if `weight_overrides` is invalid; `RuntimeError` after `max_retries + 1` failed LLM parse attempts.

## 12. Known limitations / TODOs

- FAISS and the broader RAG dependency stack are installed but not yet used by any feature — reserved for future work (e.g. richer skill matching or CV retrieval).
- `Behance`/`Dribbble` portfolio links always return `UNSUPPORTED_SOURCE`; no scraping strategy exists for them yet.
- `ComputeJDMatch.extraSkills` is always `[]` — not implemented.
- Interview context providers (`ContextProviders`) default to stubs reporting `not_available`; real candidate/job/resume/portfolio/skills data-access modules still need to be wired in for `CreateInterview` to use live data.
- All public functions return **JSON strings**, not Python dicts (`model_dump_json(...)` / `json.dumps(...)`) — callers must `json.loads()` the result.
- No persistence layer: `InterviewSession` and every other result are plain dicts passed back and forth by the caller between calls (e.g. `SubmitAnswer` expects the full session dict from the previous response) — there's no database-backed session store yet.
