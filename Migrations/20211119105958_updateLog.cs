using Microsoft.EntityFrameworkCore.Migrations;

namespace oneWin.Migrations
{
    public partial class updateLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "actionName",
                table: "log");

            migrationBuilder.DropColumn(
                name: "controllerName",
                table: "log");

            migrationBuilder.AddColumn<int>(
                name: "actionId",
                table: "log",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_log_actionId",
                table: "log",
                column: "actionId");

            migrationBuilder.AddForeignKey(
                name: "FK_log_action_actionId",
                table: "log",
                column: "actionId",
                principalTable: "action",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_log_action_actionId",
                table: "log");

            migrationBuilder.DropIndex(
                name: "IX_log_actionId",
                table: "log");

            migrationBuilder.DropColumn(
                name: "actionId",
                table: "log");

            migrationBuilder.AddColumn<string>(
                name: "actionName",
                table: "log",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "controllerName",
                table: "log",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
