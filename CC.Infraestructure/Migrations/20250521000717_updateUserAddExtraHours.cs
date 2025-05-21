using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateUserAddExtraHours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsRestricted",
                schema: "Management",
                table: "UserRestrictionShifts",
                newName: "Work");

            migrationBuilder.AddColumn<bool>(
                name: "ExtraHours",
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
                name: "ExtraHours",
                schema: "Management",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "Work",
                schema: "Management",
                table: "UserRestrictionShifts",
                newName: "IsRestricted");
        }
    }
}
