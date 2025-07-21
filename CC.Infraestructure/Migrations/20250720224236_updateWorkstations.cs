using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateWorkstations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "WorkstationCId",
                schema: "Management",
                table: "HybridWorkstations",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "WorkstationDId",
                schema: "Management",
                table: "HybridWorkstations",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HybridWorkstations_WorkstationCId",
                schema: "Management",
                table: "HybridWorkstations",
                column: "WorkstationCId");

            migrationBuilder.CreateIndex(
                name: "IX_HybridWorkstations_WorkstationDId",
                schema: "Management",
                table: "HybridWorkstations",
                column: "WorkstationDId");

            migrationBuilder.AddForeignKey(
                name: "FK_HybridWorkstations_Workstations_WorkstationCId",
                schema: "Management",
                table: "HybridWorkstations",
                column: "WorkstationCId",
                principalSchema: "Management",
                principalTable: "Workstations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HybridWorkstations_Workstations_WorkstationDId",
                schema: "Management",
                table: "HybridWorkstations",
                column: "WorkstationDId",
                principalSchema: "Management",
                principalTable: "Workstations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HybridWorkstations_Workstations_WorkstationCId",
                schema: "Management",
                table: "HybridWorkstations");

            migrationBuilder.DropForeignKey(
                name: "FK_HybridWorkstations_Workstations_WorkstationDId",
                schema: "Management",
                table: "HybridWorkstations");

            migrationBuilder.DropIndex(
                name: "IX_HybridWorkstations_WorkstationCId",
                schema: "Management",
                table: "HybridWorkstations");

            migrationBuilder.DropIndex(
                name: "IX_HybridWorkstations_WorkstationDId",
                schema: "Management",
                table: "HybridWorkstations");

            migrationBuilder.DropColumn(
                name: "WorkstationCId",
                schema: "Management",
                table: "HybridWorkstations");

            migrationBuilder.DropColumn(
                name: "WorkstationDId",
                schema: "Management",
                table: "HybridWorkstations");
        }
    }
}
