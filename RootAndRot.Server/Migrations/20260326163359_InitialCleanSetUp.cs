using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RootAndRot.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialCleanSetUp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "UsersDevices_ibfk_1",
                table: "UsersDevices");

            migrationBuilder.DropForeignKey(
                name: "UsersDevices_ibfk_2",
                table: "UsersDevices");

            migrationBuilder.DropPrimaryKey(
                name: "PRIMARY",
                table: "UsersDevices");

            migrationBuilder.DropPrimaryKey(
                name: "PRIMARY",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PRIMARY",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "C02",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "IPAddress",
                table: "Devices");

            migrationBuilder.RenameIndex(
                name: "DeviceID",
                table: "UsersDevices",
                newName: "IX_UsersDevices_DeviceID");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Users",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                collation: "latin1_swedish_ci",
                oldClrType: typeof(string),
                oldType: "varchar(60)",
                oldMaxLength: 60)
                .Annotation("MySql:CharSet", "latin1")
                .OldAnnotation("MySql:CharSet", "latin1")
                .OldAnnotation("Relational:Collation", "latin1_swedish_ci");

            migrationBuilder.AlterColumn<int>(
                name: "Methane",
                table: "Devices",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int(11)");

            migrationBuilder.AddColumn<int>(
                name: "CO2",
                table: "Devices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsersDevices",
                table: "UsersDevices",
                columns: new[] { "UserID", "DeviceID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "UserID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Devices",
                table: "Devices",
                column: "DeviceID");

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "latin1_swedish_ci")
                        .Annotation("MySql:CharSet", "latin1"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ExpiresAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Consumed = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "latin1")
                .Annotation("Relational:Collation", "latin1_swedish_ci");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersDevices_Devices_DeviceID",
                table: "UsersDevices",
                column: "DeviceID",
                principalTable: "Devices",
                principalColumn: "DeviceID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersDevices_Users_UserID",
                table: "UsersDevices",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersDevices_Devices_DeviceID",
                table: "UsersDevices");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersDevices_Users_UserID",
                table: "UsersDevices");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsersDevices",
                table: "UsersDevices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Devices",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "CO2",
                table: "Devices");

            migrationBuilder.RenameIndex(
                name: "IX_UsersDevices_DeviceID",
                table: "UsersDevices",
                newName: "DeviceID");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Users",
                type: "varchar(60)",
                maxLength: 60,
                nullable: false,
                collation: "latin1_swedish_ci",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100)
                .Annotation("MySql:CharSet", "latin1")
                .OldAnnotation("MySql:CharSet", "latin1")
                .OldAnnotation("Relational:Collation", "latin1_swedish_ci");

            migrationBuilder.AlterColumn<int>(
                name: "Methane",
                table: "Devices",
                type: "int(11)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "C02",
                table: "Devices",
                type: "int(11)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "IPAddress",
                table: "Devices",
                type: "varchar(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "",
                collation: "latin1_swedish_ci")
                .Annotation("MySql:CharSet", "latin1");

            migrationBuilder.AddPrimaryKey(
                name: "PRIMARY",
                table: "UsersDevices",
                columns: new[] { "UserID", "DeviceID" })
                .Annotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            migrationBuilder.AddPrimaryKey(
                name: "PRIMARY",
                table: "Users",
                column: "UserID");

            migrationBuilder.AddPrimaryKey(
                name: "PRIMARY",
                table: "Devices",
                column: "DeviceID");

            migrationBuilder.AddForeignKey(
                name: "UsersDevices_ibfk_1",
                table: "UsersDevices",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "UserID");

            migrationBuilder.AddForeignKey(
                name: "UsersDevices_ibfk_2",
                table: "UsersDevices",
                column: "DeviceID",
                principalTable: "Devices",
                principalColumn: "DeviceID");
        }
    }
}
