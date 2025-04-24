using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutenticacaoDoisFatores.Infra.Migrations
{
    /// <inheritdoc />
    public partial class auditorias_id_entidade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "IdEntidade",
                table: "Auditorias",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdEntidade",
                table: "Auditorias");
        }
    }
}
