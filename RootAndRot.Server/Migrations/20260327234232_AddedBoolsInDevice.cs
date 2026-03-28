using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RootAndRot.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddedBoolsInDevice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "RefreshTokenId",
                table: "RefreshTokens",
                type: "char(100)",
                maxLength: 100,
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(string),
                oldType: "char(100)",
                oldMaxLength: 100)
                .OldAnnotation("MySql:CharSet", "latin1")
                .OldAnnotation("Relational:Collation", "latin1_swedish_ci");

            migrationBuilder.AddColumn<bool>(
                name: "DoesntHaveMeatOrDairy",
                table: "Devices",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasDairy",
                table: "Devices",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasMeat",
                table: "Devices",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoesntHaveMeatOrDairy",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "HasDairy",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "HasMeat",
                table: "Devices");

            migrationBuilder.AlterColumn<string>(
                name: "RefreshTokenId",
                table: "RefreshTokens",
                type: "char(100)",
                maxLength: 100,
                nullable: false,
                collation: "latin1_swedish_ci",
                oldClrType: typeof(Guid),
                oldType: "char(100)",
                oldMaxLength: 100)
                .Annotation("MySql:CharSet", "latin1")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");
        }
    }
}
