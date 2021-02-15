using Microsoft.EntityFrameworkCore.Migrations;

namespace ModulePlayer.Migrations
{
    public partial class ExtraTrackingInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SuspendData",
                table: "TrackingData",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SuspendData",
                table: "TrackingData");
        }
    }
}
