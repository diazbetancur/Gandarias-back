using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addPropertiesUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ComplementHours",
                schema: "Management",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "HiredHours",
                schema: "Management",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "LawApply",
                schema: "Management",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ComplementHours",
                schema: "Management",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "HiredHours",
                schema: "Management",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LawApply",
                schema: "Management",
                table: "AspNetUsers");
        }
    }
}
