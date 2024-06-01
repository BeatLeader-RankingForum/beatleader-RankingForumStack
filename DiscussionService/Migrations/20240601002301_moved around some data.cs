using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscussionService.Migrations
{
    /// <inheritdoc />
    public partial class movedaroundsomedata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Discussions_MapDiscussionId_Phase",
                table: "Discussions");

            migrationBuilder.DropColumn(
                name: "CreatedOnUtc",
                table: "Discussions");

            migrationBuilder.DropColumn(
                name: "Phase",
                table: "Discussions");

            migrationBuilder.AddColumn<int>(
                name: "Phase",
                table: "MapDiscussions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Discussions_MapDiscussionId_Characteristic_Difficulty",
                table: "Discussions",
                columns: new[] { "MapDiscussionId", "Characteristic", "Difficulty" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Discussions_MapDiscussionId_Characteristic_Difficulty",
                table: "Discussions");

            migrationBuilder.DropColumn(
                name: "Phase",
                table: "MapDiscussions");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOnUtc",
                table: "Discussions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Phase",
                table: "Discussions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Discussions_MapDiscussionId_Phase",
                table: "Discussions",
                columns: new[] { "MapDiscussionId", "Phase" },
                unique: true);
        }
    }
}
