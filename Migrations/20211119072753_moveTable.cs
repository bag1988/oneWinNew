using Microsoft.EntityFrameworkCore.Migrations;

namespace oneWin.Migrations
{
    public partial class moveTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ViewSiteCost",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViewSiteCost", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ViewSiteInssue",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViewSiteInssue", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ViewSiteSections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViewSiteSections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ViewSiteValidaty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViewSiteValidaty", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ViewSiteCost");

            migrationBuilder.DropTable(
                name: "ViewSiteInssue");

            migrationBuilder.DropTable(
                name: "ViewSiteSections");

            migrationBuilder.DropTable(
                name: "ViewSiteValidaty");
        }
    }
}
