using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommentService.Migrations
{
    /// <inheritdoc />
    public partial class removePosition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Position",
                table: "OrderedThreadItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Position",
                table: "OrderedThreadItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
