using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EditConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CandidateCertifications_CandidateProfiles_CandidateId",
                table: "CandidateCertifications");

            migrationBuilder.DropForeignKey(
                name: "FK_CandidateLanguages_CandidateProfiles_CandidateId",
                table: "CandidateLanguages");

            migrationBuilder.DropForeignKey(
                name: "FK_CandidateProjects_CandidateProfiles_CandidateId",
                table: "CandidateProjects");

            migrationBuilder.DropForeignKey(
                name: "FK_Skills_SkillCategories_CategoryId",
                table: "Skills");

            migrationBuilder.DropTable(
                name: "CandidateSocialLinks");

            migrationBuilder.DropTable(
                name: "EmailVerificationTokens");

            migrationBuilder.DropTable(
                name: "PasswordResetTokens");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SkillCategories",
                table: "SkillCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CandidateProjects",
                table: "CandidateProjects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CandidateLanguages",
                table: "CandidateLanguages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CandidateCertifications",
                table: "CandidateCertifications");

            migrationBuilder.RenameTable(
                name: "SkillCategories",
                newName: "SkillCategory");

            migrationBuilder.RenameTable(
                name: "CandidateProjects",
                newName: "CandidateProject");

            migrationBuilder.RenameTable(
                name: "CandidateLanguages",
                newName: "CandidateLanguage");

            migrationBuilder.RenameTable(
                name: "CandidateCertifications",
                newName: "CandidateCertification");

            migrationBuilder.RenameIndex(
                name: "IX_CandidateProjects_CandidateId",
                table: "CandidateProject",
                newName: "IX_CandidateProject_CandidateId");

            migrationBuilder.RenameIndex(
                name: "IX_CandidateLanguages_CandidateId",
                table: "CandidateLanguage",
                newName: "IX_CandidateLanguage_CandidateId");

            migrationBuilder.RenameIndex(
                name: "IX_CandidateCertifications_CandidateId",
                table: "CandidateCertification",
                newName: "IX_CandidateCertification_CandidateId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SkillCategory",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "SkillCategory",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CandidateProject",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<string>(
                name: "ProficiencyLevel",
                table: "CandidateLanguage",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Language",
                table: "CandidateLanguage",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CandidateCertification",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SkillCategory",
                table: "SkillCategory",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CandidateProject",
                table: "CandidateProject",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CandidateLanguage",
                table: "CandidateLanguage",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CandidateCertification",
                table: "CandidateCertification",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateCertification_CandidateProfiles_CandidateId",
                table: "CandidateCertification",
                column: "CandidateId",
                principalTable: "CandidateProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_SkillCategory_CategoryId",
                table: "Skills",
                column: "CategoryId",
                principalTable: "SkillCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CandidateCertification_CandidateProfiles_CandidateId",
                table: "CandidateCertification");

            migrationBuilder.DropForeignKey(
                name: "FK_CandidateLanguage_CandidateProfiles_CandidateId",
                table: "CandidateLanguage");

            migrationBuilder.DropForeignKey(
                name: "FK_CandidateProject_CandidateProfiles_CandidateId",
                table: "CandidateProject");

            migrationBuilder.DropForeignKey(
                name: "FK_Skills_SkillCategory_CategoryId",
                table: "Skills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SkillCategory",
                table: "SkillCategory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CandidateProject",
                table: "CandidateProject");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CandidateLanguage",
                table: "CandidateLanguage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CandidateCertification",
                table: "CandidateCertification");

            migrationBuilder.RenameTable(
                name: "SkillCategory",
                newName: "SkillCategories");

            migrationBuilder.RenameTable(
                name: "CandidateProject",
                newName: "CandidateProjects");

            migrationBuilder.RenameTable(
                name: "CandidateLanguage",
                newName: "CandidateLanguages");

            migrationBuilder.RenameTable(
                name: "CandidateCertification",
                newName: "CandidateCertifications");

            migrationBuilder.RenameIndex(
                name: "IX_CandidateProject_CandidateId",
                table: "CandidateProjects",
                newName: "IX_CandidateProjects_CandidateId");

            migrationBuilder.RenameIndex(
                name: "IX_CandidateLanguage_CandidateId",
                table: "CandidateLanguages",
                newName: "IX_CandidateLanguages_CandidateId");

            migrationBuilder.RenameIndex(
                name: "IX_CandidateCertification_CandidateId",
                table: "CandidateCertifications",
                newName: "IX_CandidateCertifications_CandidateId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SkillCategories",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "SkillCategories",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CandidateProjects",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ProficiencyLevel",
                table: "CandidateLanguages",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Language",
                table: "CandidateLanguages",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CandidateCertifications",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SkillCategories",
                table: "SkillCategories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CandidateProjects",
                table: "CandidateProjects",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CandidateLanguages",
                table: "CandidateLanguages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CandidateCertifications",
                table: "CandidateCertifications",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CandidateSocialLinks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CandidateId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Platform = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateSocialLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CandidateSocialLinks_CandidateProfiles_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "CandidateProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmailVerificationTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TokenHash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UsedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailVerificationTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailVerificationTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PasswordResetTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TokenHash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UsedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordResetTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PasswordResetTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpireAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RevokeAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Token = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CandidateSocialLinks_CandidateId",
                table: "CandidateSocialLinks",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailVerificationTokens_UserId",
                table: "EmailVerificationTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetTokens_UserId",
                table: "PasswordResetTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateCertifications_CandidateProfiles_CandidateId",
                table: "CandidateCertifications",
                column: "CandidateId",
                principalTable: "CandidateProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_SkillCategories_CategoryId",
                table: "Skills",
                column: "CategoryId",
                principalTable: "SkillCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
