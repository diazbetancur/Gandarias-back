using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addfieldsData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "EndDate",
                schema: "Management",
                table: "WorkstationDemandTemplates",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<DateOnly>(
                name: "StartDate",
                schema: "Management",
                table: "WorkstationDemandTemplates",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                schema: "Management",
                table: "WorkstationDemandTemplates");

            migrationBuilder.DropColumn(
                name: "StartDate",
                schema: "Management",
                table: "WorkstationDemandTemplates");
        }
    }
}
