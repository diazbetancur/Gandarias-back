using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class hybridWorkStation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HybridWorkstations",
                schema: "Management",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    WorkstationAId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkstationBId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HybridWorkstations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HybridWorkstations_Workstations_WorkstationAId",
                        column: x => x.WorkstationAId,
                        principalSchema: "Management",
                        principalTable: "Workstations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HybridWorkstations_Workstations_WorkstationBId",
                        column: x => x.WorkstationBId,
                        principalSchema: "Management",
                        principalTable: "Workstations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HybridWorkstations_WorkstationAId",
                schema: "Management",
                table: "HybridWorkstations",
                column: "WorkstationAId");

            migrationBuilder.CreateIndex(
                name: "IX_HybridWorkstations_WorkstationBId",
                schema: "Management",
                table: "HybridWorkstations",
                column: "WorkstationBId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HybridWorkstations",
                schema: "Management");
        }
    }
}
