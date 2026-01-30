using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations;

public partial class AddEmpregadoIdControle : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<long>(
            name: "empregado_id",
            schema: "integracao_sisponto",
            table: "colaboradores",
            type: "bigint",
            nullable: false,
            defaultValue: 0L);

        migrationBuilder.AddColumn<long>(
            name: "empregado_id",
            schema: "integracao_sisponto",
            table: "afastamentos",
            type: "bigint",
            nullable: false,
            defaultValue: 0L);

        migrationBuilder.AddColumn<string>(
            name: "controle",
            schema: "integracao_sisponto",
            table: "afastamentos",
            type: "character varying(40)",
            maxLength: 40,
            nullable: false,
            defaultValue: "");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "empregado_id",
            schema: "integracao_sisponto",
            table: "colaboradores");

        migrationBuilder.DropColumn(
            name: "empregado_id",
            schema: "integracao_sisponto",
            table: "afastamentos");

        migrationBuilder.DropColumn(
            name: "controle",
            schema: "integracao_sisponto",
            table: "afastamentos");
    }
}
