using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthService.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyRefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_ExpiraEm",
                table: "RefreshTokens");

            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_UsuarioId",
                table: "RefreshTokens");

            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_UsuarioId_Revogado",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "Revogado",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "RevogadoEm",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "SubstituidoPor",
                table: "RefreshTokens");


            migrationBuilder.Sql(@"
                WITH CTE AS (
                    SELECT Id,
                           ROW_NUMBER() OVER (PARTITION BY UsuarioId ORDER BY CriadoEm DESC) AS RowNum
                    FROM RefreshTokens
                )
                DELETE FROM CTE WHERE RowNum > 1;
            ");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UsuarioId",
                table: "RefreshTokens",
                column: "UsuarioId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_UsuarioId",
                table: "RefreshTokens");

            migrationBuilder.AddColumn<bool>(
                name: "Revogado",
                table: "RefreshTokens",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "RevogadoEm",
                table: "RefreshTokens",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubstituidoPor",
                table: "RefreshTokens",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_ExpiraEm",
                table: "RefreshTokens",
                column: "ExpiraEm");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UsuarioId",
                table: "RefreshTokens",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UsuarioId_Revogado",
                table: "RefreshTokens",
                columns: new[] { "UsuarioId", "Revogado" });
        }
    }
}
