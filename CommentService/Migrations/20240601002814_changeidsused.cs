using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommentService.Migrations
{
    /// <inheritdoc />
    public partial class changeidsused : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DiscussionId",
                table: "Comments",
                newName: "MapDiscussionId");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "Comments",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "DifficultyDiscussionId",
                table: "Comments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageLink",
                table: "Comments",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DifficultyDiscussionId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "ImageLink",
                table: "Comments");

            migrationBuilder.RenameColumn(
                name: "MapDiscussionId",
                table: "Comments",
                newName: "DiscussionId");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Comments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
