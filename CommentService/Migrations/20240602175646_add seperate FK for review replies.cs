using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommentService.Migrations
{
    /// <inheritdoc />
    public partial class addseperateFKforreviewreplies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderedThreadItems_Comments_CommentId",
                table: "OrderedThreadItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderedThreadItems_Reviews_CommentId",
                table: "OrderedThreadItems");

            migrationBuilder.AlterColumn<string>(
                name: "CommentId",
                table: "OrderedThreadItems",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "ReviewId",
                table: "OrderedThreadItems",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderedThreadItems_ReviewId",
                table: "OrderedThreadItems",
                column: "ReviewId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderedThreadItems_Comments_CommentId",
                table: "OrderedThreadItems",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderedThreadItems_Reviews_ReviewId",
                table: "OrderedThreadItems",
                column: "ReviewId",
                principalTable: "Reviews",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderedThreadItems_Comments_CommentId",
                table: "OrderedThreadItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderedThreadItems_Reviews_ReviewId",
                table: "OrderedThreadItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderedThreadItems_ReviewId",
                table: "OrderedThreadItems");

            migrationBuilder.DropColumn(
                name: "ReviewId",
                table: "OrderedThreadItems");

            migrationBuilder.AlterColumn<string>(
                name: "CommentId",
                table: "OrderedThreadItems",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderedThreadItems_Comments_CommentId",
                table: "OrderedThreadItems",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderedThreadItems_Reviews_CommentId",
                table: "OrderedThreadItems",
                column: "CommentId",
                principalTable: "Reviews",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
