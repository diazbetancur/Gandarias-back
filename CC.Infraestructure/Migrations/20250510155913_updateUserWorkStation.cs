using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateUserWorkStation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Covertage",
                schema: "Management",
                table: "UserWorkstations",
                newName: "Coverage");

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                schema: "Management",
                table: "UserWorkstations",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDelete",
                schema: "Management",
                table: "UserWorkstations");

            migrationBuilder.RenameColumn(
                name: "Coverage",
                schema: "Management",
                table: "UserWorkstations",
                newName: "Covertage");
        }
    }
}
