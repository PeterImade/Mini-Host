using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Logs.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeploymentHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppInstanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PerformedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeploymentHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeploymentLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppInstanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeploymentLogs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeploymentHistories_AppInstanceId",
                table: "DeploymentHistories",
                column: "AppInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_DeploymentLogs_AppInstanceId",
                table: "DeploymentLogs",
                column: "AppInstanceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeploymentHistories");

            migrationBuilder.DropTable(
                name: "DeploymentLogs");
        }
    }
}
