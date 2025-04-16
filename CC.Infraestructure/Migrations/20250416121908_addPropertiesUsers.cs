using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addPropertiesUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "JobTitle",
                schema: "Management",
                table: "AspNetUsers",
                newName: "NickName");

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
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
                name: "IsDelete",
                schema: "Management",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "NickName",
                schema: "Management",
                table: "AspNetUsers",
                newName: "JobTitle");
        }
    }
}
