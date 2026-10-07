using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminUserSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CandidateLanguage_CandidateProfiles_CandidateId",
                table: "CandidateLanguage");

            migrationBuilder.DropForeignKey(
                name: "FK_CandidateProject_CandidateProfiles_CandidateId",
                table: "CandidateProject");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CandidateProject",
                table: "CandidateProject");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CandidateLanguage",
                table: "CandidateLanguage");

            migrationBuilder.RenameTable(
                name: "CandidateProject",
                newName: "CandidateProjects");

            migrationBuilder.RenameTable(
                name: "CandidateLanguage",
                newName: "CandidateLanguages");

            migrationBuilder.RenameIndex(
                name: "IX_CandidateProject_CandidateId",
                table: "CandidateProjects",
                newName: "IX_CandidateProjects_CandidateId");

            migrationBuilder.RenameIndex(
                name: "IX_CandidateLanguage_CandidateId",
                table: "CandidateLanguages",
                newName: "IX_CandidateLanguages_CandidateId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CandidateProjects",
                table: "CandidateProjects",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CandidateLanguages",
                table: "CandidateLanguages",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "InterviewerAvailabilities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InterviewerId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TimeZone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SlotDurationMinutes = table.Column<int>(type: "integer", nullable: false),
                    MaxBookingsPerSlot = table.Column<int>(type: "integer", nullable: false),
                    InterviewTypes = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    JobId = table.Column<Guid>(type: "uuid", nullable: true),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsRecurring = table.Column<bool>(type: "boolean", nullable: false),
                    RecurrencePattern = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    RecurrenceEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewerAvailabilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewerAvailabilities_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterviewerAvailabilities_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_InterviewerAvailabilities_Users_InterviewerId",
                        column: x => x.InterviewerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Interviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uuid", nullable: false),
                    CandidateId = table.Column<Guid>(type: "uuid", nullable: false),
                    JobId = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    InterviewerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    InterviewType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ScheduledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DurationMinutes = table.Column<int>(type: "integer", nullable: false),
                    LocationOrLink = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    Feedback = table.Column<string>(type: "text", nullable: true),
                    Rating = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Interviews_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Interviews_CandidateProfiles_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "CandidateProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Interviews_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Interviews_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Interviews_Users_InterviewerId",
                        column: x => x.InterviewerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    SenderId = table.Column<Guid>(type: "uuid", nullable: true),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: true),
                    Title = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RelatedEntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InterviewBookings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AvailabilityId = table.Column<Guid>(type: "uuid", nullable: false),
                    CandidateId = table.Column<Guid>(type: "uuid", nullable: false),
                    JobId = table.Column<Guid>(type: "uuid", nullable: true),
                    ApplicationId = table.Column<Guid>(type: "uuid", nullable: true),
                    BookedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ScheduledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DurationMinutes = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CancellationReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CancelledById = table.Column<Guid>(type: "uuid", nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LocationOrLink = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    InterviewId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewBookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewBookings_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_InterviewBookings_InterviewerAvailabilities_AvailabilityId",
                        column: x => x.AvailabilityId,
                        principalTable: "InterviewerAvailabilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterviewBookings_Interviews_Id",
                        column: x => x.Id,
                        principalTable: "Interviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_InterviewBookings_Interviews_InterviewId",
                        column: x => x.InterviewId,
                        principalTable: "Interviews",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InterviewBookings_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_InterviewBookings_Users_CancelledById",
                        column: x => x.CancelledById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_InterviewBookings_Users_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InterviewReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InterviewId = table.Column<Guid>(type: "uuid", nullable: false),
                    CandidateId = table.Column<Guid>(type: "uuid", nullable: false),
                    GeneratedById = table.Column<Guid>(type: "uuid", nullable: false),
                    ReportType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    ExecutiveSummary = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    FinalRecommendation = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    OverallScore = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FinalizedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewReports_Interviews_InterviewId",
                        column: x => x.InterviewId,
                        principalTable: "Interviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterviewReports_Users_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterviewReports_Users_GeneratedById",
                        column: x => x.GeneratedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InterviewSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InterviewId = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionNumber = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    SessionType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ScheduledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DurationMinutes = table.Column<int>(type: "integer", nullable: false),
                    LocationOrLink = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Feedback = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    Rating = table.Column<int>(type: "integer", nullable: true),
                    InterviewerId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewSessions_Interviews_InterviewId",
                        column: x => x.InterviewId,
                        principalTable: "Interviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterviewSessions_Users_InterviewerId",
                        column: x => x.InterviewerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "InterviewEvaluations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    InterviewId = table.Column<Guid>(type: "uuid", nullable: false),
                    EvaluatorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CandidateId = table.Column<Guid>(type: "uuid", nullable: false),
                    OverallScore = table.Column<int>(type: "integer", nullable: false),
                    OverallRating = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Summary = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    Strengths = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    Weaknesses = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    Recommendation = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsSubmitted = table.Column<bool>(type: "boolean", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewEvaluations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewEvaluations_InterviewSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "InterviewSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterviewEvaluations_Interviews_InterviewId",
                        column: x => x.InterviewId,
                        principalTable: "Interviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterviewEvaluations_Users_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterviewEvaluations_Users_EvaluatorId",
                        column: x => x.EvaluatorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InterviewQuestions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionText = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    QuestionType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    MaxScore = table.Column<int>(type: "integer", nullable: true),
                    ExpectedAnswer = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Difficulty = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewQuestions_InterviewSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "InterviewSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InterviewEvaluationCriteria",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EvaluationId = table.Column<Guid>(type: "uuid", nullable: false),
                    CriteriaName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CriteriaCategory = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    MaxScore = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Weight = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewEvaluationCriteria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewEvaluationCriteria_InterviewEvaluations_Evaluation~",
                        column: x => x.EvaluationId,
                        principalTable: "InterviewEvaluations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InterviewAnswers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CandidateId = table.Column<Guid>(type: "uuid", nullable: false),
                    AnswerText = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: true),
                    EvaluatorNotes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    EvaluatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    EvaluatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewAnswers_InterviewQuestions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "InterviewQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterviewAnswers_Users_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterviewAnswers_Users_EvaluatedById",
                        column: x => x.EvaluatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InterviewAnswers_CandidateId",
                table: "InterviewAnswers",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewAnswers_EvaluatedById",
                table: "InterviewAnswers",
                column: "EvaluatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewAnswers_QuestionId",
                table: "InterviewAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewBookings_ApplicationId",
                table: "InterviewBookings",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewBookings_AvailabilityId",
                table: "InterviewBookings",
                column: "AvailabilityId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewBookings_AvailabilityId_CandidateId_Status",
                table: "InterviewBookings",
                columns: new[] { "AvailabilityId", "CandidateId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_InterviewBookings_CancelledById",
                table: "InterviewBookings",
                column: "CancelledById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewBookings_CandidateId",
                table: "InterviewBookings",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewBookings_InterviewId",
                table: "InterviewBookings",
                column: "InterviewId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewBookings_JobId",
                table: "InterviewBookings",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewBookings_ScheduledAt",
                table: "InterviewBookings",
                column: "ScheduledAt");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewBookings_Status",
                table: "InterviewBookings",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewerAvailabilities_CompanyId",
                table: "InterviewerAvailabilities",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewerAvailabilities_InterviewerId",
                table: "InterviewerAvailabilities",
                column: "InterviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewerAvailabilities_InterviewerId_StartTime_EndTime",
                table: "InterviewerAvailabilities",
                columns: new[] { "InterviewerId", "StartTime", "EndTime" });

            migrationBuilder.CreateIndex(
                name: "IX_InterviewerAvailabilities_JobId",
                table: "InterviewerAvailabilities",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewerAvailabilities_StartTime",
                table: "InterviewerAvailabilities",
                column: "StartTime");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewerAvailabilities_Status",
                table: "InterviewerAvailabilities",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewEvaluationCriteria_EvaluationId",
                table: "InterviewEvaluationCriteria",
                column: "EvaluationId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewEvaluations_CandidateId",
                table: "InterviewEvaluations",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewEvaluations_EvaluatorId",
                table: "InterviewEvaluations",
                column: "EvaluatorId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewEvaluations_InterviewId",
                table: "InterviewEvaluations",
                column: "InterviewId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewEvaluations_IsSubmitted",
                table: "InterviewEvaluations",
                column: "IsSubmitted");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewEvaluations_SessionId",
                table: "InterviewEvaluations",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewQuestions_Order",
                table: "InterviewQuestions",
                column: "Order");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewQuestions_SessionId",
                table: "InterviewQuestions",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewReports_CandidateId",
                table: "InterviewReports",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewReports_GeneratedById",
                table: "InterviewReports",
                column: "GeneratedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewReports_InterviewId",
                table: "InterviewReports",
                column: "InterviewId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewReports_Status",
                table: "InterviewReports",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_ApplicationId",
                table: "Interviews",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_CandidateId",
                table: "Interviews",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_CompanyId",
                table: "Interviews",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_InterviewerId",
                table: "Interviews",
                column: "InterviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_JobId",
                table: "Interviews",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewSessions_InterviewerId",
                table: "InterviewSessions",
                column: "InterviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewSessions_InterviewId",
                table: "InterviewSessions",
                column: "InterviewId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewSessions_ScheduledAt",
                table: "InterviewSessions",
                column: "ScheduledAt");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewSessions_Status",
                table: "InterviewSessions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_SenderId",
                table: "Notifications",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateLanguages_CandidateProfiles_CandidateId",
                table: "CandidateLanguages",
                column: "CandidateId",
                principalTable: "CandidateProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateProjects_CandidateProfiles_CandidateId",
                table: "CandidateProjects",
                column: "CandidateId",
                principalTable: "CandidateProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CandidateLanguages_CandidateProfiles_CandidateId",
                table: "CandidateLanguages");

            migrationBuilder.DropForeignKey(
                name: "FK_CandidateProjects_CandidateProfiles_CandidateId",
                table: "CandidateProjects");

            migrationBuilder.DropTable(
                name: "InterviewAnswers");

            migrationBuilder.DropTable(
                name: "InterviewBookings");

            migrationBuilder.DropTable(
                name: "InterviewEvaluationCriteria");

            migrationBuilder.DropTable(
                name: "InterviewReports");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "InterviewQuestions");

            migrationBuilder.DropTable(
                name: "InterviewerAvailabilities");

            migrationBuilder.DropTable(
                name: "InterviewEvaluations");

            migrationBuilder.DropTable(
                name: "InterviewSessions");

            migrationBuilder.DropTable(
                name: "Interviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CandidateProjects",
                table: "CandidateProjects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CandidateLanguages",
                table: "CandidateLanguages");

            migrationBuilder.RenameTable(
                name: "CandidateProjects",
                newName: "CandidateProject");

            migrationBuilder.RenameTable(
                name: "CandidateLanguages",
                newName: "CandidateLanguage");

            migrationBuilder.RenameIndex(
                name: "IX_CandidateProjects_CandidateId",
                table: "CandidateProject",
                newName: "IX_CandidateProject_CandidateId");

            migrationBuilder.RenameIndex(
                name: "IX_CandidateLanguages_CandidateId",
                table: "CandidateLanguage",
                newName: "IX_CandidateLanguage_CandidateId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CandidateProject",
                table: "CandidateProject",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CandidateLanguage",
                table: "CandidateLanguage",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateLanguage_CandidateProfiles_CandidateId",
                table: "CandidateLanguage",
                column: "CandidateId",
                principalTable: "CandidateProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateProject_CandidateProfiles_CandidateId",
                table: "CandidateProject",
                column: "CandidateId",
                principalTable: "CandidateProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
