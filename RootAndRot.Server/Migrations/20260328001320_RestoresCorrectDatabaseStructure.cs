using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RootAndRot.Server.Migrations
{
    /// <inheritdoc />
    public partial class RestoresCorrectDatabaseStructure : Migration
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
