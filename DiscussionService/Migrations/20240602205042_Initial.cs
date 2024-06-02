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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MapsetId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Phase = table.Column<int>(type: "int", nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DiscussionOwnerIds = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapDiscussions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DifficultyDiscussions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MapDiscussionId = table.Column<int>(type: "int", nullable: false),
                    Characteristic = table.Column<int>(type: "int", nullable: false),
                    Difficulty = table.Column<int>(type: "int", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false)
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
                name: "IX_DifficultyDiscussions_MapDiscussionId_Characteristic_Difficulty",
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
