using Franquias.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Franquias.Api.Configurations;

/// <summary>
/// Mapeamento da entidade <see cref="Franqueadora"/>.
/// </summary>
public sealed class FranqueadoraConfiguracao : IEntityTypeConfiguration<Franqueadora>
{
    public void Configure(EntityTypeBuilder<Franqueadora> entidade)
    {
        entidade.ToTable("Franqueadoras");

        entidade.HasKey(franqueadora => franqueadora.Id);
        entidade.Property(franqueadora => franqueadora.Id).ValueGeneratedOnAdd();

        entidade.Property(franqueadora => franqueadora.RazaoSocial).HasMaxLength(150).IsRequired();
        entidade.Property(franqueadora => franqueadora.NomeFantasia).HasMaxLength(150).IsRequired();
        entidade.Property(franqueadora => franqueadora.Cnpj).HasMaxLength(14).IsRequired();
        entidade.Property(franqueadora => franqueadora.Email).HasMaxLength(150).IsRequired();
        entidade.Property(franqueadora => franqueadora.Telefone).HasMaxLength(20);
        entidade.Property(franqueadora => franqueadora.Cidade).HasMaxLength(100);
        entidade.Property(franqueadora => franqueadora.Uf).HasMaxLength(2);

        // O SQLite não possui um tipo decimal nativo. A conversão para double
        // garante que somas, comparações e ordenações funcionem nas consultas LINQ.
        entidade.Property(franqueadora => franqueadora.PercentualRoyaltyPadrao)
            .HasConversion<double>()
            .IsRequired();

        entidade.Property(franqueadora => franqueadora.Ativa).IsRequired();
        entidade.Property(franqueadora => franqueadora.CriadaEm).IsRequired();

        entidade.HasIndex(franqueadora => franqueadora.Cnpj).IsUnique();
    }
}

/// <summary>
/// Mapeamento da entidade <see cref="Franqueado"/>.
/// </summary>
public sealed class FranqueadoConfiguracao : IEntityTypeConfiguration<Franqueado>
{
    public void Configure(EntityTypeBuilder<Franqueado> entidade)
    {
        entidade.ToTable("Franqueados");

        entidade.HasKey(franqueado => franqueado.Id);
        entidade.Property(franqueado => franqueado.Id).ValueGeneratedOnAdd();

        entidade.Property(franqueado => franqueado.Nome).HasMaxLength(150).IsRequired();
        entidade.Property(franqueado => franqueado.Cpf).HasMaxLength(11).IsRequired();
        entidade.Property(franqueado => franqueado.Email).HasMaxLength(150).IsRequired();
        entidade.Property(franqueado => franqueado.Telefone).HasMaxLength(20);
        entidade.Property(franqueado => franqueado.Cidade).HasMaxLength(100);
        entidade.Property(franqueado => franqueado.Uf).HasMaxLength(2);
        entidade.Property(franqueado => franqueado.Ativo).IsRequired();
        entidade.Property(franqueado => franqueado.CriadoEm).IsRequired();

        entidade.HasIndex(franqueado => franqueado.Cpf).IsUnique();
    }
}

/// <summary>
/// Mapeamento da entidade <see cref="UnidadeFranqueada"/>.
/// </summary>
public sealed class UnidadeFranqueadaConfiguracao : IEntityTypeConfiguration<UnidadeFranqueada>
{
    public void Configure(EntityTypeBuilder<UnidadeFranqueada> entidade)
    {
        entidade.ToTable("UnidadesFranqueadas");

        entidade.HasKey(unidade => unidade.Id);
        entidade.Property(unidade => unidade.Id).ValueGeneratedOnAdd();

        entidade.Property(unidade => unidade.Codigo).HasMaxLength(20).IsRequired();
        entidade.Property(unidade => unidade.RazaoSocial).HasMaxLength(150).IsRequired();
        entidade.Property(unidade => unidade.NomeFantasia).HasMaxLength(150).IsRequired();
        entidade.Property(unidade => unidade.Cnpj).HasMaxLength(14).IsRequired();
        entidade.Property(unidade => unidade.Email).HasMaxLength(150);
        entidade.Property(unidade => unidade.Telefone).HasMaxLength(20);
        entidade.Property(unidade => unidade.Logradouro).HasMaxLength(150);
        entidade.Property(unidade => unidade.Numero).HasMaxLength(20);
        entidade.Property(unidade => unidade.Bairro).HasMaxLength(100);
        entidade.Property(unidade => unidade.Cidade).HasMaxLength(100).IsRequired();
        entidade.Property(unidade => unidade.Uf).HasMaxLength(2).IsRequired();
        entidade.Property(unidade => unidade.Cep).HasMaxLength(8);
        entidade.Property(unidade => unidade.ResponsavelNome).HasMaxLength(150);
        entidade.Property(unidade => unidade.ResponsavelTelefone).HasMaxLength(20);

        entidade.Property(unidade => unidade.PercentualRoyalty)
            .HasConversion<double>()
            .IsRequired();

        entidade.Property(unidade => unidade.Situacao)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        entidade.Property(unidade => unidade.DataInicioContrato).IsRequired();
        entidade.Property(unidade => unidade.CriadaEm).IsRequired();

        // Regra de negócio: não é permitido cadastrar duas unidades com o mesmo CNPJ.
        entidade.HasIndex(unidade => unidade.Cnpj).IsUnique();
        entidade.HasIndex(unidade => unidade.Codigo).IsUnique();
        entidade.HasIndex(unidade => unidade.Situacao);
        entidade.HasIndex(unidade => unidade.Cidade);

        entidade.HasOne(unidade => unidade.Franqueadora)
            .WithMany(franqueadora => franqueadora.Unidades)
            .HasForeignKey(unidade => unidade.FranqueadoraId)
            .OnDelete(DeleteBehavior.Restrict);

        entidade.HasOne(unidade => unidade.Franqueado)
            .WithMany(franqueado => franqueado.Unidades)
            .HasForeignKey(unidade => unidade.FranqueadoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
