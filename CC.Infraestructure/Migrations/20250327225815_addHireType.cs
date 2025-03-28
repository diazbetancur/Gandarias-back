using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addHireType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_HireType_HireTypeId",
                schema: "Management",
                table: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HireType",
                schema: "Management",
                table: "HireType");

            migrationBuilder.RenameTable(
                name: "HireType",
                schema: "Management",
                newName: "HireTypes",
                newSchema: "Management");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreated",
                schema: "Management",
                table: "HireTypes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "Management",
                table: "HireTypes",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HireTypes",
                schema: "Management",
                table: "HireTypes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_HireTypes_HireTypeId",
                schema: "Management",
                table: "AspNetUsers",
                column: "HireTypeId",
                principalSchema: "Management",
                principalTable: "HireTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_HireTypes_HireTypeId",
                schema: "Management",
                table: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HireTypes",
                schema: "Management",
                table: "HireTypes");

            migrationBuilder.RenameTable(
                name: "HireTypes",
                schema: "Management",
                newName: "HireType",
                newSchema: "Management");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreated",
                schema: "Management",
                table: "HireType",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "Management",
                table: "HireType",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HireType",
                schema: "Management",
                table: "HireType",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_HireType_HireTypeId",
                schema: "Management",
                table: "AspNetUsers",
                column: "HireTypeId",
                principalSchema: "Management",
                principalTable: "HireType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
