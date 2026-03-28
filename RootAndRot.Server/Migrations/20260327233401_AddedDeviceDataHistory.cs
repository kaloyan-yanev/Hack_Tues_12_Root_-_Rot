using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RootAndRot.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddedDeviceDataHistory : Migration
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

            migrationBuilder.CreateTable(
                name: "DeviceDataSet",
                columns: table => new
                {
                    DataID = table.Column<Guid>(type: "char(36)", nullable: false, defaultValueSql: "(UUID())", collation: "ascii_general_ci"),
                    DeviceId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    Temperature = table.Column<int>(type: "int", nullable: false),
                    Humidity = table.Column<int>(type: "int", nullable: false),
                    CO2 = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceDataSet", x => x.DataID);
                    table.ForeignKey(
                        name: "FK_DeviceDataSet_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "DeviceID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "latin1")
                .Annotation("Relational:Collation", "latin1_swedish_ci");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceDataSet_DeviceId",
                table: "DeviceDataSet",
                column: "DeviceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceDataSet");

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
