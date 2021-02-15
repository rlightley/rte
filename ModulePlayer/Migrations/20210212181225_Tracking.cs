using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ModulePlayer.Migrations
{
    public partial class Tracking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TrackingDataId",
                table: "Modules",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TrackingData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Progress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Complete = table.Column<bool>(type: "bit", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackingData", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Modules_TrackingDataId",
                table: "Modules",
                column: "TrackingDataId");

            migrationBuilder.AddForeignKey(
                name: "FK_Modules_TrackingData_TrackingDataId",
                table: "Modules",
                column: "TrackingDataId",
                principalTable: "TrackingData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Modules_TrackingData_TrackingDataId",
                table: "Modules");

            migrationBuilder.DropTable(
                name: "TrackingData");

            migrationBuilder.DropIndex(
                name: "IX_Modules_TrackingDataId",
                table: "Modules");

            migrationBuilder.DropColumn(
                name: "TrackingDataId",
                table: "Modules");
        }
    }
}
