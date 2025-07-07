using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateShiftType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubType",
                schema: "Management",
                table: "ShiftTypes");

            migrationBuilder.RenameColumn(
                name: "IsFlexibleExit",
                schema: "Management",
                table: "ShiftTypes",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "Block2End",
                schema: "Management",
                table: "ShiftTypes",
                newName: "Block2lastStart");

            migrationBuilder.RenameColumn(
                name: "Block1End",
                schema: "Management",
                table: "ShiftTypes",
                newName: "Block1lastStart");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsActive",
                schema: "Management",
                table: "ShiftTypes",
                newName: "IsFlexibleExit");

            migrationBuilder.RenameColumn(
                name: "Block2lastStart",
                schema: "Management",
                table: "ShiftTypes",
                newName: "Block2End");

            migrationBuilder.RenameColumn(
                name: "Block1lastStart",
                schema: "Management",
                table: "ShiftTypes",
                newName: "Block1End");

            migrationBuilder.AddColumn<int>(
                name: "SubType",
                schema: "Management",
                table: "ShiftTypes",
                type: "integer",
                nullable: true);
        }
    }
}
