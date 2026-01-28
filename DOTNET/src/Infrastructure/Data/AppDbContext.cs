using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Colaborador> Colaboradores => Set<Colaborador>();
    public DbSet<Afastamento> Afastamentos => Set<Afastamento>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("integracao_sisponto");

        modelBuilder.Entity<Colaborador>(entity =>
        {
            entity.ToTable("colaboradores");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Matricula).IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Matricula).HasColumnName("matricula").HasMaxLength(13).IsRequired();
            entity.Property(e => e.Nome).HasColumnName("nome").HasMaxLength(50).IsRequired();
            entity.Property(e => e.Pis).HasColumnName("pis").HasMaxLength(11).IsRequired();
            entity.Property(e => e.Cpf).HasColumnName("cpf").HasMaxLength(11).IsRequired();
            entity.Property(e => e.DataAdmissao).HasColumnName("data_admissao").HasColumnType("date").IsRequired();
            entity.Property(e => e.DataDemissao).HasColumnName("data_demissao").HasColumnType("date");
            entity.Property(e => e.NomeSetor).HasColumnName("nome_setor").HasMaxLength(40);
            entity.Property(e => e.NomeCargo).HasColumnName("nome_cargo").HasMaxLength(40);
            entity.Property(e => e.NomeDepartamento).HasColumnName("nome_departamento").HasMaxLength(40);
            entity.Property(e => e.NomeUnidade).HasColumnName("nome_unidade").HasMaxLength(40).IsRequired();
            entity.Property(e => e.NomeLotacao).HasColumnName("nome_lotacao").HasMaxLength(40).IsRequired();
            entity.Property(e => e.Situacao).HasColumnName("situacao").HasMaxLength(10).IsRequired();
            entity.Property(e => e.CnpjUnidade).HasColumnName("cnpj_unidade").HasMaxLength(14).IsRequired();
            entity.Property(e => e.Ctps).HasColumnName("ctps").HasMaxLength(15);
            entity.Property(e => e.Serie).HasColumnName("serie").HasMaxLength(15);

            entity.HasMany(e => e.Afastamentos)
                .WithOne(e => e.Colaborador)
                .HasForeignKey(e => e.Matricula)
                .HasPrincipalKey(e => e.Matricula);
        });

        modelBuilder.Entity<Afastamento>(entity =>
        {
            entity.ToTable("afastamentos");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Matricula).HasColumnName("matricula").HasMaxLength(13).IsRequired();
            entity.Property(e => e.Descricao).HasColumnName("descricao").HasMaxLength(50).IsRequired();
            entity.Property(e => e.DataInicio).HasColumnName("data_inicio").IsRequired();
            entity.Property(e => e.DataFinal).HasColumnName("data_final");
            entity.Property(e => e.CnpjUnidade).HasColumnName("cnpj_unidade").HasMaxLength(14);
            entity.Property(e => e.CodigoSituacao).HasColumnName("codigo_situacao").HasMaxLength(10);
        });
    }
}
