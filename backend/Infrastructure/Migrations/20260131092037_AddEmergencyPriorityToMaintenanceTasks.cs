using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroTrack.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmergencyPriorityToMaintenanceTasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEmergency",
                table: "MaintenanceTasks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "MaintenanceTasks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEmergency",
                table: "MaintenanceTasks");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "MaintenanceTasks");
        }
    }
}
