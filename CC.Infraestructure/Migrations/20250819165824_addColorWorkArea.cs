using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addColorWorkArea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                schema: "Management",
                table: "WorkAreas",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                schema: "Management",
                table: "WorkAreas");
        }
    }
}
