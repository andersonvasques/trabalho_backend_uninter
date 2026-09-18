using Franquias.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Data;

/// <summary>
/// Representa a conexão da aplicação com o banco de dados relacional.
///
/// O DbContext é responsável por consultar, adicionar, atualizar e excluir
/// registros, além de acompanhar as alterações feitas nas entidades.
/// </summary>
public sealed class AppDbContext : DbContext
{
    /// <summary>
    /// As configurações do banco chegam por injeção de dependência
    /// (registradas em Program.cs com AddDbContext).
    /// </summary>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>Tabela de usuários do sistema.</summary>
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    /// <summary>Tabela da franqueadora (matriz da rede).</summary>
    public DbSet<Franqueadora> Franqueadoras => Set<Franqueadora>();

    /// <summary>Tabela dos franqueados.</summary>
    public DbSet<Franqueado> Franqueados => Set<Franqueado>();

    /// <summary>Tabela das unidades franqueadas.</summary>
    public DbSet<UnidadeFranqueada> Unidades => Set<UnidadeFranqueada>();

    /// <summary>Tabela de categorias do catálogo.</summary>
    public DbSet<Categoria> Categorias => Set<Categoria>();

    /// <summary>Tabela de produtos e serviços.</summary>
    public DbSet<ProdutoServico> ProdutosServicos => Set<ProdutoServico>();

    /// <summary>Tabela de fornecedores homologados.</summary>
    public DbSet<Fornecedor> Fornecedores => Set<Fornecedor>();

    /// <summary>Tabela de associação entre produtos e fornecedores.</summary>
    public DbSet<ProdutoFornecedor> ProdutosFornecedores => Set<ProdutoFornecedor>();

    /// <summary>Tabela de saldos de estoque por unidade.</summary>
    public DbSet<EstoqueUnidade> EstoquesUnidades => Set<EstoqueUnidade>();

    /// <summary>Tabela do histórico de movimentações de estoque.</summary>
    public DbSet<MovimentacaoEstoque> MovimentacoesEstoque => Set<MovimentacaoEstoque>();

    /// <summary>Tabela de vendas.</summary>
    public DbSet<Venda> Vendas => Set<Venda>();

    /// <summary>Tabela dos itens das vendas.</summary>
    public DbSet<ItemVenda> ItensVenda => Set<ItemVenda>();

    /// <summary>Tabela das cobranças de royalties.</summary>
    public DbSet<Royalty> Royalties => Set<Royalty>();

    /// <summary>Tabela dos chamados de suporte.</summary>
    public DbSet<ChamadoSuporte> ChamadosSuporte => Set<ChamadoSuporte>();

    /// <summary>Tabela do histórico de interações dos chamados.</summary>
    public DbSet<ChamadoInteracao> ChamadosInteracoes => Set<ChamadoInteracao>();

    /// <summary>
    /// Aplica todas as classes de configuração encontradas no projeto
    /// (pasta Configurations), mantendo o mapeamento separado das entidades.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
