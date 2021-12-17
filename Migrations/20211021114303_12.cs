using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace oneWin.Migrations
{
    public partial class _12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "roleController",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    idController = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roleController", x => x.id);
                    table.ForeignKey(
                        name: "FK_roleController_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_roleController_controller_idController",
                        column: x => x.idController,
                        principalTable: "controller",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "roleAction",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    idAction = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roleAction", x => x.id);
                    table.ForeignKey(
                        name: "FK_roleAction_action_idAction",
                        column: x => x.idAction,
                        principalTable: "action",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_roleAction_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

           
            migrationBuilder.CreateIndex(
                name: "IX_roleAction_idAction",
                table: "roleAction",
                column: "idAction");

            migrationBuilder.CreateIndex(
                name: "IX_roleAction_RoleId",
                table: "roleAction",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_roleController_idController",
                table: "roleController",
                column: "idController");

            migrationBuilder.CreateIndex(
                name: "IX_roleController_RoleId",
                table: "roleController",
                column: "RoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}
