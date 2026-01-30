using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations;

public partial class AddDataMovimento : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTime>(
            name: "datamovimento",
            schema: "integracao_sisponto",
            table: "colaboradores",
            type: "timestamp without time zone",
            nullable: false,
            defaultValueSql: "now()");

        migrationBuilder.AddColumn<DateTime>(
            name: "datamovimento",
            schema: "integracao_sisponto",
            table: "afastamentos",
            type: "timestamp without time zone",
            nullable: false,
            defaultValueSql: "now()");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "datamovimento",
            schema: "integracao_sisponto",
            table: "colaboradores");

        migrationBuilder.DropColumn(
            name: "datamovimento",
            schema: "integracao_sisponto",
            table: "afastamentos");
    }
}
