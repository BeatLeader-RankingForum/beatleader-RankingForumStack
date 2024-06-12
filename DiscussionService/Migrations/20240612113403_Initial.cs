using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscussionService.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MapDiscussions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    MapsetId = table.Column<string>(type: "text", nullable: false),
                    Phase = table.Column<int>(type: "integer", nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DiscussionOwnerIds = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapDiscussions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DifficultyDiscussions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    MapDiscussionId = table.Column<string>(type: "text", nullable: false),
                    Characteristic = table.Column<int>(type: "integer", nullable: false),
                    Difficulty = table.Column<int>(type: "integer", nullable: false),
                    IsLocked = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DifficultyDiscussions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DifficultyDiscussions_MapDiscussions_MapDiscussionId",
                        column: x => x.MapDiscussionId,
                        principalTable: "MapDiscussions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DifficultyDiscussions_MapDiscussionId_Characteristic_Diffic~",
                table: "DifficultyDiscussions",
                columns: new[] { "MapDiscussionId", "Characteristic", "Difficulty" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MapDiscussions_MapsetId",
                table: "MapDiscussions",
                column: "MapsetId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DifficultyDiscussions");

            migrationBuilder.DropTable(
                name: "MapDiscussions");
        }
    }
}
