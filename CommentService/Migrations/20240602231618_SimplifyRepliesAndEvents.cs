using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommentService.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyRepliesAndEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderedThreadItems_Comments_CommentId",
                table: "OrderedThreadItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderedThreadItems_Reviews_ReviewId",
                table: "OrderedThreadItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderedThreadItems",
                table: "OrderedThreadItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderedThreadItems_ReviewId",
                table: "OrderedThreadItems");

            migrationBuilder.DropColumn(
                name: "Body",
                table: "OrderedThreadItems");

            migrationBuilder.DropColumn(
                name: "ItemType",
                table: "OrderedThreadItems");

            migrationBuilder.RenameTable(
                name: "OrderedThreadItems",
                newName: "StatusUpdates");

            migrationBuilder.RenameIndex(
                name: "IX_OrderedThreadItems_CommentId",
                table: "StatusUpdates",
                newName: "IX_StatusUpdates_CommentId");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "StatusUpdates",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ReviewId",
                table: "StatusUpdates",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_StatusUpdates",
                table: "StatusUpdates",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Replies",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AuthorId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommentId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ReviewId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsEdited = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Replies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Replies_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Replies_Reviews_ReviewId",
                        column: x => x.ReviewId,
                        principalTable: "Reviews",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Replies_CommentId",
                table: "Replies",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Replies_ReviewId",
                table: "Replies",
                column: "ReviewId");

            migrationBuilder.AddForeignKey(
                name: "FK_StatusUpdates_Comments_CommentId",
                table: "StatusUpdates",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StatusUpdates_Comments_CommentId",
                table: "StatusUpdates");

            migrationBuilder.DropTable(
                name: "Replies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StatusUpdates",
                table: "StatusUpdates");

            migrationBuilder.RenameTable(
                name: "StatusUpdates",
                newName: "OrderedThreadItems");

            migrationBuilder.RenameIndex(
                name: "IX_StatusUpdates_CommentId",
                table: "OrderedThreadItems",
                newName: "IX_OrderedThreadItems_CommentId");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "OrderedThreadItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "ReviewId",
                table: "OrderedThreadItems",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Body",
                table: "OrderedThreadItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemType",
                table: "OrderedThreadItems",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderedThreadItems",
                table: "OrderedThreadItems",
                column: "Id");

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
    }
}
