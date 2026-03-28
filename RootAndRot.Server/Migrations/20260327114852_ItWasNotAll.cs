using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RootAndRot.Server.Migrations
{
    /// <inheritdoc />
    public partial class ItWasNotAll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Devices");

            migrationBuilder.RenameColumn(
                name: "Hum_Threshold",
                table: "Devices",
                newName: "HumThreshold");

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

            migrationBuilder.AlterColumn<float>(
                name: "Temp_Threshold",
                table: "Devices",
                type: "float",
                nullable: false,
                defaultValueSql: "30",
                oldClrType: typeof(float),
                oldType: "float");

            migrationBuilder.AlterColumn<string>(
                name: "MACAddress",
                table: "Devices",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                collation: "latin1_swedish_ci",
                oldClrType: typeof(string),
                oldType: "char(17)",
                oldFixedLength: true,
                oldMaxLength: 17)
                .Annotation("MySql:CharSet", "latin1")
                .OldAnnotation("MySql:CharSet", "latin1")
                .OldAnnotation("Relational:Collation", "latin1_swedish_ci");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HumThreshold",
                table: "Devices",
                newName: "Hum_Threshold");

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

            migrationBuilder.AlterColumn<float>(
                name: "Temp_Threshold",
                table: "Devices",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "float",
                oldDefaultValueSql: "30");

            migrationBuilder.AlterColumn<string>(
                name: "MACAddress",
                table: "Devices",
                type: "char(17)",
                fixedLength: true,
                maxLength: 17,
                nullable: false,
                collation: "latin1_swedish_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50)
                .Annotation("MySql:CharSet", "latin1")
                .OldAnnotation("MySql:CharSet", "latin1")
                .OldAnnotation("Relational:Collation", "latin1_swedish_ci");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Devices",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                collation: "latin1_swedish_ci")
                .Annotation("MySql:CharSet", "latin1");
        }
    }
}
