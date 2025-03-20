using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class configurePermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkstationWorkAreas",
                schema: "Management");

            migrationBuilder.AddColumn<Guid>(
                name: "WorkAreaId",
                schema: "Management",
                table: "Workstations",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "HireDate",
                schema: "Management",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "JobTitle",
                schema: "Management",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "WorkstationId",
                schema: "Management",
                table: "AspNetUsers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserWorkstations",
                schema: "Management",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    UserId1 = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkstationId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserWorkstations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserWorkstations_AspNetUsers_UserId1",
                        column: x => x.UserId1,
                        principalSchema: "Management",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserWorkstations_Workstations_WorkstationId",
                        column: x => x.WorkstationId,
                        principalSchema: "Management",
                        principalTable: "Workstations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Workstations_WorkAreaId",
                schema: "Management",
                table: "Workstations",
                column: "WorkAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_WorkstationId",
                schema: "Management",
                table: "AspNetUsers",
                column: "WorkstationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserWorkstations_UserId1",
                schema: "Management",
                table: "UserWorkstations",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_UserWorkstations_WorkstationId",
                schema: "Management",
                table: "UserWorkstations",
                column: "WorkstationId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Workstations_WorkstationId",
                schema: "Management",
                table: "AspNetUsers",
                column: "WorkstationId",
                principalSchema: "Management",
                principalTable: "Workstations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Workstations_WorkAreas_WorkAreaId",
                schema: "Management",
                table: "Workstations",
                column: "WorkAreaId",
                principalSchema: "Management",
                principalTable: "WorkAreas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Workstations_WorkstationId",
                schema: "Management",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Workstations_WorkAreas_WorkAreaId",
                schema: "Management",
                table: "Workstations");

            migrationBuilder.DropTable(
                name: "UserWorkstations",
                schema: "Management");

            migrationBuilder.DropIndex(
                name: "IX_Workstations_WorkAreaId",
                schema: "Management",
                table: "Workstations");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_WorkstationId",
                schema: "Management",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "WorkAreaId",
                schema: "Management",
                table: "Workstations");

            migrationBuilder.DropColumn(
                name: "HireDate",
                schema: "Management",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "JobTitle",
                schema: "Management",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "WorkstationId",
                schema: "Management",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "WorkstationWorkAreas",
                schema: "Management",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    WorkAreaId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkstationId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkstationWorkAreas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkstationWorkAreas_WorkAreas_WorkAreaId",
                        column: x => x.WorkAreaId,
                        principalSchema: "Management",
                        principalTable: "WorkAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkstationWorkAreas_Workstations_WorkstationId",
                        column: x => x.WorkstationId,
                        principalSchema: "Management",
                        principalTable: "Workstations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkstationWorkAreas_WorkAreaId",
                schema: "Management",
                table: "WorkstationWorkAreas",
                column: "WorkAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkstationWorkAreas_WorkstationId",
                schema: "Management",
                table: "WorkstationWorkAreas",
                column: "WorkstationId");
        }
    }
}
