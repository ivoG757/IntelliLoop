using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntelliLoop.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenamedKeyConceptsToConcepts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "KeyConcepts",
                table: "Lectures",
                newName: "Concepts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Concepts",
                table: "Lectures",
                newName: "KeyConcepts");
        }
    }
}
