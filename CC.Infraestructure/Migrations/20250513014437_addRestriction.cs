using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addRestriction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserRestrictionShifts",
                schema: "Management",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Observation = table.Column<string>(type: "text", nullable: false),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    IsRestricted = table.Column<bool>(type: "boolean", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ShiftTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserRestrictionShifts",
                schema: "Management");
        }
    }
}
