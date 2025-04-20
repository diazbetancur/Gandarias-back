using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class deteleOther : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShiftSchedules",
                schema: "Management");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "Block1End",
                schema: "Management",
                table: "ShiftTypes",
                type: "interval",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "Block1Start",
                schema: "Management",
                table: "ShiftTypes",
                type: "interval",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "Block2End",
                schema: "Management",
                table: "ShiftTypes",
                type: "interval",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "Block2Start",
                schema: "Management",
                table: "ShiftTypes",
                type: "interval",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Block1End",
                schema: "Management",
                table: "ShiftTypes");

            migrationBuilder.DropColumn(
                name: "Block1Start",
                schema: "Management",
                table: "ShiftTypes");

            migrationBuilder.DropColumn(
                name: "Block2End",
                schema: "Management",
                table: "ShiftTypes");

            migrationBuilder.DropColumn(
                name: "Block2Start",
                schema: "Management",
                table: "ShiftTypes");

            migrationBuilder.CreateTable(
                name: "ShiftSchedules",
                schema: "Management",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    ShiftTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Block1End = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Block1Start = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Block2End = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Block2Start = table.Column<TimeSpan>(type: "interval", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShiftSchedules_ShiftTypes_ShiftTypeId",
                        column: x => x.ShiftTypeId,
                        principalSchema: "Management",
                        principalTable: "ShiftTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShiftSchedules_ShiftTypeId",
                schema: "Management",
                table: "ShiftSchedules",
                column: "ShiftTypeId");
        }
    }
}
