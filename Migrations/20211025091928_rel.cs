using Microsoft.EntityFrameworkCore.Migrations;

namespace oneWin.Migrations
{
    public partial class rel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "nameController",
                table: "controller",
                newName: "addressController");

            migrationBuilder.RenameColumn(
                name: "nameAction",
                table: "action",
                newName: "addressAction");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "addressController",
                table: "controller",
                newName: "nameController");

            migrationBuilder.RenameColumn(
                name: "addressAction",
                table: "action",
                newName: "nameAction");
        }
    }
}
