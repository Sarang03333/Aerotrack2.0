using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroTrack.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LinkServiceEventToTask : Migration
    {
        /// <inheritdoc />
       protected override void Up(MigrationBuilder migrationBuilder)
{
    // Add column only if missing
    migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.ServiceEvents','TaskId') IS NULL
    ALTER TABLE dbo.ServiceEvents ADD TaskId nvarchar(450) NULL;
");

    // Create filtered unique index only if missing
    migrationBuilder.Sql(@"
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes 
    WHERE name = 'IX_ServiceEvents_TaskId' 
      AND object_id = OBJECT_ID('dbo.ServiceEvents')
)
    CREATE UNIQUE INDEX IX_ServiceEvents_TaskId 
    ON dbo.ServiceEvents(TaskId) WHERE TaskId IS NOT NULL;
");
}

// Down can stay as-is or guard similarly; optional to adjust

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop FK if exists
            migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_ServiceEvents_MaintenanceTasks_TaskId')
    ALTER TABLE dbo.ServiceEvents DROP CONSTRAINT FK_ServiceEvents_MaintenanceTasks_TaskId;
");

            // Drop index if exists
            migrationBuilder.Sql(@"
IF EXISTS (
    SELECT 1 FROM sys.indexes 
    WHERE name = 'IX_ServiceEvents_TaskId' 
      AND object_id = OBJECT_ID('dbo.ServiceEvents')
)
    DROP INDEX IX_ServiceEvents_TaskId ON dbo.ServiceEvents;
");

            // Drop column if exists
            migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.ServiceEvents','TaskId') IS NOT NULL
    ALTER TABLE dbo.ServiceEvents DROP COLUMN TaskId;
");
        }
    }
}
