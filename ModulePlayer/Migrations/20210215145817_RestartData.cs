using Microsoft.EntityFrameworkCore.Migrations;

namespace ModulePlayer.Migrations
{
    public partial class RestartData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompletedTimes",
                table: "Modules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RestartedTimes",
                table: "Modules",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedTimes",
                table: "Modules");

            migrationBuilder.DropColumn(
                name: "RestartedTimes",
                table: "Modules");
        }
    }
}
