using Microsoft.EntityFrameworkCore.Migrations;

namespace oneWin.Migrations
{
    public partial class addOtdelLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "otdelName",
                table: "log",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "otdelName",
                table: "log");
        }
    }
}
