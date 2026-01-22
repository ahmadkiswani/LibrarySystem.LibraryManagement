using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibrarySystem.Migrations
{
    /// <inheritdoc />
    public partial class InitClean : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PublisherId",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AuthorId",
                table: "BookCopies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "BookCopies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Books_PublisherId",
                table: "Books",
                column: "PublisherId");

            migrationBuilder.CreateIndex(
                name: "IX_BookCopies_AuthorId",
                table: "BookCopies",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_BookCopies_CategoryId",
                table: "BookCopies",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookCopies_Authors_AuthorId",
                table: "BookCopies",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookCopies_Categories_CategoryId",
                table: "BookCopies",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Publishers_PublisherId",
                table: "Books",
                column: "PublisherId",
                principalTable: "Publishers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookCopies_Authors_AuthorId",
                table: "BookCopies");

            migrationBuilder.DropForeignKey(
                name: "FK_BookCopies_Categories_CategoryId",
                table: "BookCopies");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_Publishers_PublisherId",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_PublisherId",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_BookCopies_AuthorId",
                table: "BookCopies");

            migrationBuilder.DropIndex(
                name: "IX_BookCopies_CategoryId",
                table: "BookCopies");

            migrationBuilder.DropColumn(
                name: "PublisherId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "BookCopies");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "BookCopies");
        }
    }
}
