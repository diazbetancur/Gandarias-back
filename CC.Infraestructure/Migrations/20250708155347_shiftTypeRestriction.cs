using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class shiftTypeRestriction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserRestrictionShifts",
                schema: "Management");

            migrationBuilder.CreateTable(
                name: "EmployeeShiftTypeRestrictions",
                schema: "Management",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DayOfWeek = table.Column<int>(type: "integer", nullable: false),
                    ShiftTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeShiftTypeRestrictions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeShiftTypeRestrictions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "Management",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeShiftTypeRestrictions_ShiftTypes_ShiftTypeId",
                        column: x => x.ShiftTypeId,
                        principalSchema: "Management",
                        principalTable: "ShiftTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeShiftTypeRestrictions_ShiftTypeId",
                schema: "Management",
                table: "EmployeeShiftTypeRestrictions",
                column: "ShiftTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeShiftTypeRestrictions_UserId",
                schema: "Management",
                table: "EmployeeShiftTypeRestrictions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeShiftTypeRestrictions",
                schema: "Management");

            migrationBuilder.CreateTable(
                name: "UserRestrictionShifts",
                schema: "Management",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    ShiftTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    Observation = table.Column<string>(type: "text", nullable: false),
                    Work = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRestrictionShifts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRestrictionShifts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "Management",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRestrictionShifts_ShiftTypes_ShiftTypeId",
                        column: x => x.ShiftTypeId,
                        principalSchema: "Management",
                        principalTable: "ShiftTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserRestrictionShifts_ShiftTypeId",
                schema: "Management",
                table: "UserRestrictionShifts",
                column: "ShiftTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRestrictionShifts_UserId",
                schema: "Management",
                table: "UserRestrictionShifts",
                column: "UserId");
        }
    }
}
