using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate_BO : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WFCaseLinks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SourceCaseId = table.Column<long>(type: "INTEGER", nullable: false),
                    TargetCaseId = table.Column<long>(type: "INTEGER", nullable: true),
                    LinkType = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedByUserId = table.Column<long>(type: "INTEGER", nullable: false),
                    SourceMainEntityId = table.Column<long>(type: "INTEGER", nullable: false),
                    TargetMainEntityId = table.Column<long>(type: "INTEGER", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    SourceAppId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TargetAppId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SourceMainEntityName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    TargetMainEntityName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    SourceWFClassName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    TargetWFClassName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ProcessMetaDataJson = table.Column<byte[]>(type: "BLOB", nullable: true),
                    EntityJson = table.Column<string>(type: "TEXT", nullable: false),
                    currentTaskId = table.Column<long>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WFCaseLinks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowEntity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EntityJson = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowEntity", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WFCaseLinks");

            migrationBuilder.DropTable(
                name: "WorkflowEntity");
        }
    }
}
