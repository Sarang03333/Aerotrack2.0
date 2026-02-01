using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroTrack.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditSeverity_ComplianceAuto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Severity",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Severity",
                table: "AuditLogs");
        }
    }
}
