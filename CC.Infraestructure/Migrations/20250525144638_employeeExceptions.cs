using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class employeeExceptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmployeeScheduleExceptions",
                schema: "Management",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    RestrictionType = table.Column<int>(type: "integer", nullable: false),
                    IsAdditionalRestriction = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("PK_EmployeeScheduleExceptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeScheduleExceptions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "Management",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeScheduleExceptions_UserId",
                schema: "Management",
                table: "EmployeeScheduleExceptions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeScheduleExceptions",
                schema: "Management");
        }
    }
}
