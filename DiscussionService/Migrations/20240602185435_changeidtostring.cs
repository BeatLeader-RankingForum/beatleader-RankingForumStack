using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscussionService.Migrations
{
    /// <inheritdoc />
    public partial class changeidtostring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Discussions");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "MapDiscussions",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.CreateTable(
                name: "DifficultyDiscussions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MapDiscussionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DifficultyDiscussions");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "MapDiscussions",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.CreateTable(
                name: "Discussions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Characteristic = table.Column<int>(type: "int", nullable: false),
                    Difficulty = table.Column<int>(type: "int", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    MapDiscussionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discussions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Discussions_MapDiscussions_MapDiscussionId",
                        column: x => x.MapDiscussionId,
                        principalTable: "MapDiscussions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Discussions_MapDiscussionId_Characteristic_Difficulty",
                table: "Discussions",
                columns: new[] { "MapDiscussionId", "Characteristic", "Difficulty" },
                unique: true);
        }
    }
}
