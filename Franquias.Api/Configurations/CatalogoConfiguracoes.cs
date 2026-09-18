using Franquias.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Franquias.Api.Configurations;

/// <summary>
/// Mapeamento da entidade <see cref="Categoria"/>.
/// </summary>
public sealed class CategoriaConfiguracao : IEntityTypeConfiguration<Categoria>
{
    public void Configure(EntityTypeBuilder<Categoria> entidade)
    {
        entidade.ToTable("Categorias");

        entidade.HasKey(categoria => categoria.Id);
        entidade.Property(categoria => categoria.Id).ValueGeneratedOnAdd();

        entidade.Property(categoria => categoria.Nome).HasMaxLength(100).IsRequired();
        entidade.Property(categoria => categoria.Descricao).HasMaxLength(300);
        entidade.Property(categoria => categoria.Ativa).IsRequired();
        entidade.Property(categoria => categoria.CriadaEm).IsRequired();

        entidade.HasIndex(categoria => categoria.Nome).IsUnique();
    }
}

/// <summary>
/// Mapeamento da entidade <see cref="ProdutoServico"/>.
/// </summary>
public sealed class ProdutoServicoConfiguracao : IEntityTypeConfiguration<ProdutoServico>
{
    public void Configure(EntityTypeBuilder<ProdutoServico> entidade)
    {
        entidade.ToTable("ProdutosServicos");

        entidade.HasKey(produto => produto.Id);
        entidade.Property(produto => produto.Id).ValueGeneratedOnAdd();

        entidade.Property(produto => produto.Sku).HasMaxLength(30).IsRequired();
        entidade.Property(produto => produto.Nome).HasMaxLength(150).IsRequired();
        entidade.Property(produto => produto.Descricao).HasMaxLength(500);

        entidade.Property(produto => produto.Tipo)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        entidade.Property(produto => produto.PrecoBase)
            .HasConversion<double>()
            .IsRequired();

        entidade.Property(produto => produto.EstoqueMinimoPadrao).IsRequired();
        entidade.Property(produto => produto.Ativo).IsRequired();
        entidade.Property(produto => produto.CriadoEm).IsRequired();

        entidade.HasIndex(produto => produto.Sku).IsUnique();
        entidade.HasIndex(produto => produto.Nome);

        entidade.HasOne(produto => produto.Categoria)
            .WithMany(categoria => categoria.Itens)
            .HasForeignKey(produto => produto.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

/// <summary>
/// Mapeamento da entidade <see cref="Fornecedor"/>.
/// </summary>
public sealed class FornecedorConfiguracao : IEntityTypeConfiguration<Fornecedor>
{
    public void Configure(EntityTypeBuilder<Fornecedor> entidade)
    {
        entidade.ToTable("Fornecedores");

        entidade.HasKey(fornecedor => fornecedor.Id);
        entidade.Property(fornecedor => fornecedor.Id).ValueGeneratedOnAdd();

        entidade.Property(fornecedor => fornecedor.RazaoSocial).HasMaxLength(150).IsRequired();
        entidade.Property(fornecedor => fornecedor.NomeFantasia).HasMaxLength(150);
        entidade.Property(fornecedor => fornecedor.Cnpj).HasMaxLength(14).IsRequired();
        entidade.Property(fornecedor => fornecedor.Email).HasMaxLength(150);
        entidade.Property(fornecedor => fornecedor.Telefone).HasMaxLength(20);
        entidade.Property(fornecedor => fornecedor.Cidade).HasMaxLength(100);
        entidade.Property(fornecedor => fornecedor.Uf).HasMaxLength(2);
        entidade.Property(fornecedor => fornecedor.Ativo).IsRequired();
        entidade.Property(fornecedor => fornecedor.CriadoEm).IsRequired();

        entidade.HasIndex(fornecedor => fornecedor.Cnpj).IsUnique();
        entidade.HasIndex(fornecedor => fornecedor.RazaoSocial);
    }
}

/// <summary>
/// Mapeamento da associação muitos-para-muitos entre produtos e fornecedores.
/// </summary>
public sealed class ProdutoFornecedorConfiguracao : IEntityTypeConfiguration<ProdutoFornecedor>
{
    public void Configure(EntityTypeBuilder<ProdutoFornecedor> entidade)
    {
        entidade.ToTable("ProdutosFornecedores");

        // Chave primária composta.
        entidade.HasKey(vinculo => new { vinculo.ProdutoServicoId, vinculo.FornecedorId });

        entidade.Property(vinculo => vinculo.PrecoCusto)
            .HasConversion<double>()
            .IsRequired();

        entidade.Property(vinculo => vinculo.PrazoEntregaDias).IsRequired();
        entidade.Property(vinculo => vinculo.Preferencial).IsRequired();
        entidade.Property(vinculo => vinculo.CriadaEm).IsRequired();

        entidade.HasOne(vinculo => vinculo.ProdutoServico)
            .WithMany(produto => produto.Fornecedores)
            .HasForeignKey(vinculo => vinculo.ProdutoServicoId)
            .OnDelete(DeleteBehavior.Cascade);

        entidade.HasOne(vinculo => vinculo.Fornecedor)
            .WithMany(fornecedor => fornecedor.Produtos)
            .HasForeignKey(vinculo => vinculo.FornecedorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
