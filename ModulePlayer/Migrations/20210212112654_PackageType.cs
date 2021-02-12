using Microsoft.EntityFrameworkCore.Migrations;

namespace ModulePlayer.Migrations
{
    public partial class PackageType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Packagetype",
                table: "Modules",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Packagetype",
                table: "Modules");
        }
    }
}
