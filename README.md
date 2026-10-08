# Talentry AI System

AI backend for **Talentry**, a recruiting / candidate-evaluation platform. It turns raw candidate and job data (CVs, portfolios, job descriptions) into structured, scored, and explainable outputs that power profile building, resume scoring, portfolio review, AI interviews, skill-gap analysis, and an overall career-readiness report.

Built as a single Colab notebook (`Talentry_AI_System.ipynb`), organized as one feature per cell, with every public function typed, validated against a Pydantic schema, and documented with a `DATA CONTRACT` docstring (inputs, outputs, error cases, and an example call/response) so it can be dropped behind an API layer with minimal translation.

## Features

| Feature | Entry point | What it does |
|---|---|---|
| Candidate Profile Extraction | `ExtractCandidateProfile` | Free text (CV / bio / form) → structured `CandidateProfile` |
| Profile Quality Evaluation | `ProfileEvaluation` | Scores a `CandidateProfile` for completeness & quality |
| Resume / ATS Scoring | `ResumeEvaluation` | Scores a `.pdf`/`.docx` resume (overall, ATS, grammar, formatting) with section-level feedback |
| CV ↔ JD Match | `ComputeJDMatch` | Deterministic TF-IDF + keyword match percentage between a resume and a job description |
| Job Description Parsing | `IntakeJobDescription` / `ParseJobDescription` | Raw job posting → structured requirements, skills, keywords |
| Portfolio Analysis | `PortfolioEvaluation` | Scrapes a GitHub profile or personal website and scores design, content, and recruiter-readiness |
| AI Interview Engine | `CreateInterview`, `SubmitAnswer`, `EndInterview` | Adaptive, multi-turn interview (HR / Behavioral / Technical / Situational) with per-answer feedback and a final report |
| Skill Gap Engine | `AnalyzeSkillGap` | Compares a candidate against a role/JD across every available signal; returns missing/weak skills with priority |
| Career Readiness Orchestrator | `EvaluateCareerReadiness` | Aggregates all evaluator outputs into one weighted, cross-source readiness score |

Every LLM call is routed through a single `llm` object (`LLMRouter`) with an automatic fallback model, and every structured-output call retries against a validation error before failing (`invoke_with_schema`).

## Stack

- **LLM**: Groq (`ChatGroq`) — `openai/gpt-oss-20b` primary, `openai/gpt-oss-120b` fallback
- **Orchestration**: LangChain (`PromptTemplate`, `PydanticOutputParser`)
- **Schemas / validation**: Pydantic v2
- **Resume parsing**: `pypdf`, `docx2txt`
- **Deterministic scoring**: `scikit-learn` (TF-IDF + cosine similarity)
- **Portfolio scraping**: `requests`, `BeautifulSoup`, GitHub REST API

## Setup

1. Open `Talentry_AI_System.ipynb` in Google Colab (or adapt Cell 1 for local use).
2. Set a `GROQ_API_KEY` — in Colab, add it to the Secrets manager; locally, `export GROQ_API_KEY=...` and skip Cell 1's `userdata.get` call.
3. Run cells top to bottom. Cell 3 defines all data models used by every feature; Cell 10 overrides the interview-engine draft from Cell 3 with the version the notebook actually runs.

## Usage

```python
# 1. Extract a structured profile from raw CV text
profile = json.loads(ExtractCandidateProfile(cv_text))

# 2. Parse a job description once, reuse everywhere
jd = json.loads(IntakeJobDescription(jd_text))

# 3. Score the resume against it
cv_result = json.loads(ResumeEvaluation("/path/to/resume.pdf", description=jd))

# 4. Review the candidate's portfolio
portfolio_result = json.loads(PortfolioEvaluation("https://github.com/username", target_role=jd["job_title"]))

# 5. Find skill gaps
gap = json.loads(AnalyzeSkillGap(profile, job_description=jd, resume_evaluation=cv_result))

# 6. Roll everything into one readiness score
readiness = json.loads(EvaluateCareerReadiness(
    profile, job_description=jd,
    resume_evaluation=cv_result, portfolio_evaluation=portfolio_result,
    skill_gap_result=gap,
))
```

See `DOCUMENTATION.md` for the full data contract (inputs, outputs, error cases) of every function.

## Notes / known limitations

- Behance and Dribbble portfolio links are recognized but return `UNSUPPORTED_SOURCE` — no automated extraction exists for them yet.
- The interview engine's context providers (`load_candidate`, `load_job`, `load_resume`, `load_portfolio`, `load_skills`) default to stubs that report `not_available` until real data-access modules are wired in.
- `ComputeJDMatch`'s `extraSkills` field is currently always empty (not yet implemented).
- All functions return JSON strings (`model_dump_json` / `json.dumps`), not raw dicts — parse with `json.loads()` before use.
# AI backend

Authenticated AI endpoints are available under `/api/v1/ai`. Users need the `ai.use` permission (seeded for the built-in roles).

- `POST /api/v1/ai/analyze/{type}` runs an analysis synchronously.
- `POST /api/v1/ai/jobs/{type}` stores and queues an analysis, returning `202 Accepted` and a status URL.
- `GET /api/v1/ai/jobs/{id}` returns the caller's job status and result.

Supported types: `cv-analysis`, `portfolio-analysis`, `job-description-analysis`, `ai-interview` (question plan generation), `interview-evaluation`, `career-analysis`, and `matching`. Send input as JSON; include CV/profile/portfolio text and relevant job or interview context in that JSON.

The AI provider uses the OpenAI-compatible chat completions API. Configure `AI:BaseUrl` (for example, an OpenAI-compatible `/v1` endpoint), `AI:Model`, and `AI:ApiKey` where required. In deployment, provide secrets through environment variables such as `AI__ApiKey`; do not commit provider keys. Without provider configuration, synchronous requests return `503` and queued requests transition to `Failed` with a configuration error.

The application applies pending database migrations at startup, including `AddAiAnalysisJobs`. The API response content is generated by the configured provider and should be reviewed by a human for consequential hiring decisions.
