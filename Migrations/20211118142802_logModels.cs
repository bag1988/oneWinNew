using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace oneWin.Migrations
{
    public partial class logModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "log",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    dateRequest = table.Column<DateTime>(type: "datetime2", nullable: false),
                    controllerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    actionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    urlRequest = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    queryRequest = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_log", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {           
            migrationBuilder.DropTable(
                name: "log");
        }
    }
}
