using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addUserAbsenteeism : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserAbsenteeisms",
                schema: "Management",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AbsenteeismTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Observation = table.Column<string>(type: "text", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAbsenteeisms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAbsenteeisms_AbsenteeismTypes_AbsenteeismTypeId",
                        column: x => x.AbsenteeismTypeId,
                        principalSchema: "Management",
                        principalTable: "AbsenteeismTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserAbsenteeisms_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "Management",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserAbsenteeisms_AbsenteeismTypeId",
                schema: "Management",
                table: "UserAbsenteeisms",
                column: "AbsenteeismTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAbsenteeisms_UserId",
                schema: "Management",
                table: "UserAbsenteeisms",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserAbsenteeisms",
                schema: "Management");
        }
    }
}
