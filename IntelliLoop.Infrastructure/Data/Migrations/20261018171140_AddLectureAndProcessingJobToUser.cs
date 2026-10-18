using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntelliLoop.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLectureAndProcessingJobToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Lectures_LectureId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_LectureId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LectureId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ProcessingJobs",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Lectures",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessingJobs_UserId",
                table: "ProcessingJobs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Lectures_UserId",
                table: "Lectures",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lectures_AspNetUsers_UserId",
                table: "Lectures",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProcessingJobs_AspNetUsers_UserId",
                table: "ProcessingJobs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lectures_AspNetUsers_UserId",
                table: "Lectures");

            migrationBuilder.DropForeignKey(
                name: "FK_ProcessingJobs_AspNetUsers_UserId",
                table: "ProcessingJobs");

            migrationBuilder.DropIndex(
                name: "IX_ProcessingJobs_UserId",
                table: "ProcessingJobs");

            migrationBuilder.DropIndex(
                name: "IX_Lectures_UserId",
                table: "Lectures");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ProcessingJobs");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Lectures");

            migrationBuilder.AddColumn<Guid>(
                name: "LectureId",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_LectureId",
                table: "AspNetUsers",
                column: "LectureId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Lectures_LectureId",
                table: "AspNetUsers",
                column: "LectureId",
                principalTable: "Lectures",
                principalColumn: "Id");
        }
    }
}
