using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntelliLoop.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixLectureProcessingJobRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProcessingJobs_LectureId",
                table: "ProcessingJobs");

            migrationBuilder.AddColumn<Guid>(
                name: "ProcessingJobId",
                table: "Lectures",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProcessingJobs_LectureId",
                table: "ProcessingJobs",
                column: "LectureId",
                unique: true,
                filter: "[LectureId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProcessingJobs_LectureId",
                table: "ProcessingJobs");

            migrationBuilder.DropColumn(
                name: "ProcessingJobId",
                table: "Lectures");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessingJobs_LectureId",
                table: "ProcessingJobs",
                column: "LectureId");
        }
    }
}
