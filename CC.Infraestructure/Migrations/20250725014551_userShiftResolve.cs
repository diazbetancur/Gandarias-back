using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class userShiftResolve : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreated",
                schema: "Management",
                table: "Licenses",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "Management",
                table: "Licenses",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.CreateTable(
                name: "UserShifts",
                schema: "Management",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Day = table.Column<int>(type: "integer", nullable: false),
                    Structure = table.Column<int>(type: "integer", nullable: false),
                    Block1Start = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Block1lastStart = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Block2Start = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Block2lastStart = table.Column<TimeSpan>(type: "interval", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserShifts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserShifts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "Management",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserShifts_UserId",
                schema: "Management",
                table: "UserShifts",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserShifts",
                schema: "Management");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreated",
                schema: "Management",
                table: "Licenses",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "Management",
                table: "Licenses",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");
        }
    }
}
