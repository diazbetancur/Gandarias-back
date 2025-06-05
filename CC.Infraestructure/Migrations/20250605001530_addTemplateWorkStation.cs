using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addTemplateWorkStation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkstationDemandTemplates",
                schema: "Management",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkstationDemandTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkstationDemands",
                schema: "Management",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    TemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkstationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Day = table.Column<int>(type: "integer", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    EffortRequired = table.Column<double>(type: "double precision", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkstationDemands", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkstationDemands_WorkstationDemandTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalSchema: "Management",
                        principalTable: "WorkstationDemandTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkstationDemands_Workstations_WorkstationId",
                        column: x => x.WorkstationId,
                        principalSchema: "Management",
                        principalTable: "Workstations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkstationDemands_TemplateId",
                schema: "Management",
                table: "WorkstationDemands",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkstationDemands_WorkstationId",
                schema: "Management",
                table: "WorkstationDemands",
                column: "WorkstationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkstationDemands",
                schema: "Management");

            migrationBuilder.DropTable(
                name: "WorkstationDemandTemplates",
                schema: "Management");
        }
    }
}
