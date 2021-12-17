using Microsoft.EntityFrameworkCore.Migrations;

namespace oneWin.Migrations
{
    public partial class rel1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "nameController",
                table: "controller",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "nameAction",
                table: "action",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "nameController",
                table: "controller");

            migrationBuilder.DropColumn(
                name: "nameAction",
                table: "action");
        }
    }
}
