using Franquias.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Franquias.Api.Configurations;

/// <summary>
/// Mapeamento da cobrança de royalties por unidade e competência.
/// </summary>
public sealed class RoyaltyConfiguracao : IEntityTypeConfiguration<Royalty>
{
    public void Configure(EntityTypeBuilder<Royalty> entidade)
    {
        entidade.ToTable("Royalties");

        entidade.HasKey(royalty => royalty.Id);
        entidade.Property(royalty => royalty.Id).ValueGeneratedOnAdd();

        entidade.Property(royalty => royalty.Ano).IsRequired();
        entidade.Property(royalty => royalty.Mes).IsRequired();

        entidade.Property(royalty => royalty.FaturamentoBase)
            .HasConversion<double>()
            .IsRequired();

        entidade.Property(royalty => royalty.PercentualAplicado)
            .HasConversion<double>()
            .IsRequired();

        entidade.Property(royalty => royalty.ValorDevido)
            .HasConversion<double>()
            .IsRequired();

        entidade.Property(royalty => royalty.ValorPago)
            .HasConversion<double>()
            .IsRequired();

        entidade.Property(royalty => royalty.Situacao)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        entidade.Property(royalty => royalty.DataVencimento).IsRequired();
        entidade.Property(royalty => royalty.GeradoEm).IsRequired();

        // Uma unidade possui uma única cobrança por competência.
        entidade.HasIndex(royalty => new { royalty.UnidadeFranqueadaId, royalty.Ano, royalty.Mes })
            .IsUnique();

        entidade.HasIndex(royalty => royalty.Situacao);

        entidade.HasOne(royalty => royalty.Unidade)
            .WithMany(unidade => unidade.Royalties)
            .HasForeignKey(royalty => royalty.UnidadeFranqueadaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
