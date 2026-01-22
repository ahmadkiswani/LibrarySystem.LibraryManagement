using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibrarySystem.Migrations
{
    /// <inheritdoc />
    public partial class AddBorrowStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOverdue",
                table: "Borrows");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Borrows",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Borrows");

            migrationBuilder.AddColumn<bool>(
                name: "IsOverdue",
                table: "Borrows",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
