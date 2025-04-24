using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutenticacaoDoisFatores.Infra.Migrations
{
    /// <inheritdoc />
    public partial class auditoria_inclusao_tabela_data : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Data",
                table: "Auditorias",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Tabela",
                table: "Auditorias",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Data",
                table: "Auditorias");

            migrationBuilder.DropColumn(
                name: "Tabela",
                table: "Auditorias");
        }
    }
}
