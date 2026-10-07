using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace infrastructure.Migrations
{
    public partial class AddApplicationUniqueIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Clean up existing duplicate active applications: keep the newest by CreatedDate and mark older ones REJECTED
            migrationBuilder.Sql(
                "UPDATE \"Applications\" SET \"Status\" = 'REJECTED' WHERE \"Id\" IN (" +
                "SELECT id FROM (SELECT \"Id\" as id, row_number() OVER (PARTITION BY \"CandidateId\", \"JobId\" ORDER BY \"CreatedDate\" DESC) as rn " +
                "FROM \"Applications\" WHERE \"Status\" NOT IN ('WITHDRAWN','REJECTED')) t WHERE t.rn > 1)"
            );

            // Create a partial unique index to prevent duplicate active applications per candidate+job
            migrationBuilder.Sql("CREATE UNIQUE INDEX IF NOT EXISTS ux_applications_candidate_job_active ON \"Applications\" (\"CandidateId\", \"JobId\") WHERE \"Status\" NOT IN ('WITHDRAWN','REJECTED')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP INDEX IF EXISTS ux_applications_candidate_job_active");
        }
    }
}
