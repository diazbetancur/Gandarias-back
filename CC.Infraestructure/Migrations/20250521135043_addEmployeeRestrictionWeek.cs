using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addEmployeeRestrictionWeek : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmployeeScheduleRestrictions",
                schema: "Management",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DayOfWeek = table.Column<int>(type: "integer", nullable: false),
                    RestrictionType = table.Column<int>(type: "integer", nullable: false),
                    AvailableFrom = table.Column<TimeSpan>(type: "interval", nullable: true),
                    AvailableUntil = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Block1Start = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Block1End = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Block2Start = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Block2End = table.Column<TimeSpan>(type: "interval", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeScheduleRestrictions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeScheduleRestrictions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "Management",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeScheduleRestrictions_UserId",
                schema: "Management",
                table: "EmployeeScheduleRestrictions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeScheduleRestrictions",
                schema: "Management");
        }
    }
}
