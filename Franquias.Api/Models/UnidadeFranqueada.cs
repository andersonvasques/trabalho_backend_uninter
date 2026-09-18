namespace Franquias.Api.Models;

/// <summary>
/// Loja/unidade operada por um franqueado.
/// É a entidade central do sistema: vendas, estoque, royalties e chamados
/// sempre pertencem a uma unidade.
/// </summary>
public sealed class UnidadeFranqueada
{
    /// <summary>Identificador único da unidade.</summary>
    public int Id { get; set; }

    /// <summary>Código interno da unidade na rede (ex.: "UN-001").</summary>
    public string Codigo { get; set; } = string.Empty;

    /// <summary>Franqueadora à qual a unidade pertence.</summary>
    public int FranqueadoraId { get; set; }

    /// <summary>Dados da franqueadora.</summary>
    public Franqueadora Franqueadora { get; set; } = null!;

    /// <summary>Franqueado responsável pela unidade.</summary>
    public int FranqueadoId { get; set; }

    /// <summary>Dados do franqueado.</summary>
    public Franqueado Franqueado { get; set; } = null!;

    /// <summary>Razão social da unidade.</summary>
    public string RazaoSocial { get; set; } = string.Empty;

    /// <summary>Nome fantasia da unidade.</summary>
    public string NomeFantasia { get; set; } = string.Empty;

    /// <summary>CNPJ da unidade (somente dígitos). Não pode se repetir.</summary>
    public string Cnpj { get; set; } = string.Empty;

    /// <summary>E-mail de contato da unidade.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Telefone de contato da unidade.</summary>
    public string Telefone { get; set; } = string.Empty;

    /// <summary>Logradouro do endereço.</summary>
    public string Logradouro { get; set; } = string.Empty;

    /// <summary>Número do endereço.</summary>
    public string Numero { get; set; } = string.Empty;

    /// <summary>Bairro do endereço.</summary>
    public string Bairro { get; set; } = string.Empty;

    /// <summary>Cidade da unidade.</summary>
    public string Cidade { get; set; } = string.Empty;

    /// <summary>Unidade federativa da unidade.</summary>
    public string Uf { get; set; } = string.Empty;

    /// <summary>CEP do endereço (somente dígitos).</summary>
    public string Cep { get; set; } = string.Empty;

    /// <summary>Nome do responsável operacional pela unidade.</summary>
    public string ResponsavelNome { get; set; } = string.Empty;

    /// <summary>Telefone do responsável operacional.</summary>
    public string ResponsavelTelefone { get; set; } = string.Empty;

    /// <summary>Data de início do contrato de franquia.</summary>
    public DateTime DataInicioContrato { get; set; }

    /// <summary>Data de encerramento do contrato, quando houver.</summary>
    public DateTime? DataEncerramentoContrato { get; set; }

    /// <summary>Percentual de royalty aplicado sobre o faturamento (ex.: 5,00 = 5%).</summary>
    public decimal PercentualRoyalty { get; set; }

    /// <summary>Situação contratual da unidade.</summary>
    public SituacaoUnidade Situacao { get; set; } = SituacaoUnidade.EmImplantacao;

    /// <summary>Data de criação do cadastro.</summary>
    public DateTime CriadaEm { get; set; }

    /// <summary>Data da última alteração do cadastro.</summary>
    public DateTime? AtualizadaEm { get; set; }

    /// <summary>Usuários vinculados à unidade.</summary>
    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();

    /// <summary>Saldos de estoque da unidade.</summary>
    public ICollection<EstoqueUnidade> Estoques { get; set; } = new List<EstoqueUnidade>();

    /// <summary>Vendas registradas pela unidade.</summary>
    public ICollection<Venda> Vendas { get; set; } = new List<Venda>();

    /// <summary>Royalties apurados para a unidade.</summary>
    public ICollection<Royalty> Royalties { get; set; } = new List<Royalty>();

    /// <summary>Chamados abertos pela unidade.</summary>
    public ICollection<ChamadoSuporte> Chamados { get; set; } = new List<ChamadoSuporte>();

    /// <summary>
    /// Regra de negócio: somente unidades ativas podem registrar novas vendas.
    /// </summary>
    public bool PodeRegistrarVenda()
    {
        return Situacao == SituacaoUnidade.Ativa;
    }

    /// <summary>
    /// Inativa a unidade preservando o histórico (exclusão lógica).
    /// </summary>
    public void Inativar()
    {
        Situacao = SituacaoUnidade.Inativa;
        DataEncerramentoContrato ??= DateTime.UtcNow;
        AtualizadaEm = DateTime.UtcNow;
    }
}
