using infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace infrastructure.Migrations;

[DbContext(typeof(AppDBcontext))]
[Migration("20261008120000_AddAiAnalysisJobs")]
public sealed class AddAiAnalysisJobs : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "AiAnalysisJobs",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<Guid>(type: "uuid", nullable: false),
                Type = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                InputJson = table.Column<string>(type: "text", nullable: false),
                Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                Result = table.Column<string>(type: "text", nullable: true),
                Error = table.Column<string>(type: "text", nullable: true),
                CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_AiAnalysisJobs", x => x.Id));

        migrationBuilder.CreateIndex(name: "IX_AiAnalysisJobs_UserId_CreatedDate", table: "AiAnalysisJobs", columns: new[] { "UserId", "CreatedDate" });
    }

    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable(name: "AiAnalysisJobs");
}
