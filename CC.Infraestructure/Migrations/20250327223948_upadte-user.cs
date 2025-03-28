using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class upadteuser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Workstations_WorkstationId",
                schema: "Management",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "WorkstationId",
                schema: "Management",
                table: "AspNetUsers",
                newName: "HireTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_WorkstationId",
                schema: "Management",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_HireTypeId");

            migrationBuilder.CreateTable(
                name: "HireType",
                schema: "Management",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HireType", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_HireType_HireTypeId",
                schema: "Management",
                table: "AspNetUsers",
                column: "HireTypeId",
                principalSchema: "Management",
                principalTable: "HireType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_HireType_HireTypeId",
                schema: "Management",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "HireType",
                schema: "Management");

            migrationBuilder.RenameColumn(
                name: "HireTypeId",
                schema: "Management",
                table: "AspNetUsers",
                newName: "WorkstationId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_HireTypeId",
                schema: "Management",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_WorkstationId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Workstations_WorkstationId",
                schema: "Management",
                table: "AspNetUsers",
                column: "WorkstationId",
                principalSchema: "Management",
                principalTable: "Workstations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
