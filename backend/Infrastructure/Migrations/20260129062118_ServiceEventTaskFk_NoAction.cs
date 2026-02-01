using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroTrack.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ServiceEventTaskFk_NoAction : Migration
    {
        /// <inheritdoc />
       protected override void Up(MigrationBuilder migrationBuilder)
{
    // Only add the FK if it doesn't already exist
    migrationBuilder.Sql(@"
IF NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys 
    WHERE name = 'FK_ServiceEvents_MaintenanceTasks_TaskId'
)
    ALTER TABLE dbo.ServiceEvents
    ADD CONSTRAINT FK_ServiceEvents_MaintenanceTasks_TaskId
    FOREIGN KEY (TaskId)
    REFERENCES dbo.MaintenanceTasks(TaskId)
    ON DELETE NO ACTION;
");
}

protected override void Down(MigrationBuilder migrationBuilder)
{
    // Only drop the FK if it exists
    migrationBuilder.Sql(@"
IF EXISTS (
    SELECT 1 FROM sys.foreign_keys 
    WHERE name = 'FK_ServiceEvents_MaintenanceTasks_TaskId'
)
    ALTER TABLE dbo.ServiceEvents 
    DROP CONSTRAINT FK_ServiceEvents_MaintenanceTasks_TaskId;
");
}
    }
}
