using Franquias.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Franquias.Api.Configurations;

/// <summary>
/// Mapeamento do saldo de estoque por unidade e produto.
/// </summary>
public sealed class EstoqueUnidadeConfiguracao : IEntityTypeConfiguration<EstoqueUnidade>
{
    public void Configure(EntityTypeBuilder<EstoqueUnidade> entidade)
    {
        entidade.ToTable("EstoquesUnidades");

        entidade.HasKey(estoque => estoque.Id);
        entidade.Property(estoque => estoque.Id).ValueGeneratedOnAdd();

        entidade.Property(estoque => estoque.Quantidade).IsRequired();
        entidade.Property(estoque => estoque.QuantidadeMinima).IsRequired();
        entidade.Property(estoque => estoque.AtualizadoEm).IsRequired();

        // Cada produto possui um único saldo por unidade.
        entidade.HasIndex(estoque => new { estoque.UnidadeFranqueadaId, estoque.ProdutoServicoId })
            .IsUnique();

        entidade.HasOne(estoque => estoque.Unidade)
            .WithMany(unidade => unidade.Estoques)
            .HasForeignKey(estoque => estoque.UnidadeFranqueadaId)
            .OnDelete(DeleteBehavior.Cascade);

        entidade.HasOne(estoque => estoque.ProdutoServico)
            .WithMany(produto => produto.Estoques)
            .HasForeignKey(estoque => estoque.ProdutoServicoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

/// <summary>
/// Mapeamento do histórico de movimentações de estoque.
/// </summary>
public sealed class MovimentacaoEstoqueConfiguracao : IEntityTypeConfiguration<MovimentacaoEstoque>
{
    public void Configure(EntityTypeBuilder<MovimentacaoEstoque> entidade)
    {
        entidade.ToTable("MovimentacoesEstoque");

        entidade.HasKey(movimentacao => movimentacao.Id);
        entidade.Property(movimentacao => movimentacao.Id).ValueGeneratedOnAdd();

        entidade.Property(movimentacao => movimentacao.Tipo)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        entidade.Property(movimentacao => movimentacao.Quantidade).IsRequired();
        entidade.Property(movimentacao => movimentacao.SaldoAnterior).IsRequired();
        entidade.Property(movimentacao => movimentacao.SaldoAtual).IsRequired();
        entidade.Property(movimentacao => movimentacao.Motivo).HasMaxLength(300);
        entidade.Property(movimentacao => movimentacao.OcorridaEm).IsRequired();

        entidade.HasIndex(movimentacao => movimentacao.OcorridaEm);

        entidade.HasOne(movimentacao => movimentacao.EstoqueUnidade)
            .WithMany(estoque => estoque.Movimentacoes)
            .HasForeignKey(movimentacao => movimentacao.EstoqueUnidadeId)
            .OnDelete(DeleteBehavior.Cascade);

        entidade.HasOne(movimentacao => movimentacao.Venda)
            .WithMany()
            .HasForeignKey(movimentacao => movimentacao.VendaId)
            .OnDelete(DeleteBehavior.SetNull);

        entidade.HasOne(movimentacao => movimentacao.Usuario)
            .WithMany()
            .HasForeignKey(movimentacao => movimentacao.UsuarioId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
