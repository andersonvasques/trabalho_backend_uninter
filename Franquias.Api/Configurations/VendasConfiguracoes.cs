using Franquias.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Franquias.Api.Configurations;

/// <summary>
/// Mapeamento da entidade <see cref="Venda"/>.
/// </summary>
public sealed class VendaConfiguracao : IEntityTypeConfiguration<Venda>
{
    public void Configure(EntityTypeBuilder<Venda> entidade)
    {
        entidade.ToTable("Vendas");

        entidade.HasKey(venda => venda.Id);
        entidade.Property(venda => venda.Id).ValueGeneratedOnAdd();

        entidade.Property(venda => venda.Numero).HasMaxLength(20).IsRequired();

        entidade.Property(venda => venda.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        entidade.Property(venda => venda.FormaPagamento)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        entidade.Property(venda => venda.ValorTotal)
            .HasConversion<double>()
            .IsRequired();

        entidade.Property(venda => venda.Desconto)
            .HasConversion<double>()
            .IsRequired();

        entidade.Property(venda => venda.Cliente).HasMaxLength(150);
        entidade.Property(venda => venda.Observacao).HasMaxLength(300);
        entidade.Property(venda => venda.MotivoCancelamento).HasMaxLength(300);
        entidade.Property(venda => venda.DataVenda).IsRequired();
        entidade.Property(venda => venda.CriadaEm).IsRequired();

        entidade.HasIndex(venda => venda.Numero).IsUnique();
        entidade.HasIndex(venda => venda.DataVenda);
        entidade.HasIndex(venda => new { venda.UnidadeFranqueadaId, venda.DataVenda });

        entidade.HasOne(venda => venda.Unidade)
            .WithMany(unidade => unidade.Vendas)
            .HasForeignKey(venda => venda.UnidadeFranqueadaId)
            .OnDelete(DeleteBehavior.Restrict);

        entidade.HasOne(venda => venda.Usuario)
            .WithMany()
            .HasForeignKey(venda => venda.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

/// <summary>
/// Mapeamento da entidade <see cref="ItemVenda"/>.
/// </summary>
public sealed class ItemVendaConfiguracao : IEntityTypeConfiguration<ItemVenda>
{
    public void Configure(EntityTypeBuilder<ItemVenda> entidade)
    {
        entidade.ToTable("ItensVenda");

        entidade.HasKey(item => item.Id);
        entidade.Property(item => item.Id).ValueGeneratedOnAdd();

        entidade.Property(item => item.DescricaoItem).HasMaxLength(150).IsRequired();
        entidade.Property(item => item.Quantidade).IsRequired();

        entidade.Property(item => item.PrecoUnitario)
            .HasConversion<double>()
            .IsRequired();

        entidade.Property(item => item.Subtotal)
            .HasConversion<double>()
            .IsRequired();

        entidade.HasOne(item => item.Venda)
            .WithMany(venda => venda.Itens)
            .HasForeignKey(item => item.VendaId)
            .OnDelete(DeleteBehavior.Cascade);

        entidade.HasOne(item => item.ProdutoServico)
            .WithMany(produto => produto.ItensVendidos)
            .HasForeignKey(item => item.ProdutoServicoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
