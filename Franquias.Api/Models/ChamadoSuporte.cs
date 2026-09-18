using Franquias.Api.Common;

namespace Franquias.Api.Models;

/// <summary>
/// Solicitação aberta por uma unidade franqueada para a franqueadora.
/// </summary>
public sealed class ChamadoSuporte
{
    /// <summary>Identificador único do chamado.</summary>
    public int Id { get; set; }

    /// <summary>Protocolo do chamado (ex.: "CH-2026-000001").</summary>
    public string Protocolo { get; set; } = string.Empty;

    /// <summary>Unidade que abriu o chamado.</summary>
    public int UnidadeFranqueadaId { get; set; }

    /// <summary>Dados da unidade.</summary>
    public UnidadeFranqueada Unidade { get; set; } = null!;

    /// <summary>Usuário que abriu o chamado.</summary>
    public int UsuarioAberturaId { get; set; }

    /// <summary>Dados do usuário que abriu o chamado.</summary>
    public Usuario UsuarioAbertura { get; set; } = null!;

    /// <summary>Assunto resumido do chamado.</summary>
    public string Titulo { get; set; } = string.Empty;

    /// <summary>Descrição detalhada do problema ou solicitação.</summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>Categoria do chamado.</summary>
    public CategoriaChamado Categoria { get; set; } = CategoriaChamado.Outros;

    /// <summary>Prioridade de atendimento.</summary>
    public PrioridadeChamado Prioridade { get; set; } = PrioridadeChamado.Media;

    /// <summary>Situação atual do chamado.</summary>
    public StatusChamado Status { get; set; } = StatusChamado.Aberto;

    /// <summary>Data de abertura.</summary>
    public DateTime AbertoEm { get; set; }

    /// <summary>Data da última atualização.</summary>
    public DateTime? AtualizadoEm { get; set; }

    /// <summary>Data do encerramento.</summary>
    public DateTime? FechadoEm { get; set; }

    /// <summary>Solução registrada no encerramento.</summary>
    public string? SolucaoAplicada { get; set; }

    /// <summary>Histórico de interações do chamado.</summary>
    public ICollection<ChamadoInteracao> Interacoes { get; set; } = new List<ChamadoInteracao>();

    /// <summary>
    /// Indica se o chamado ainda está em andamento.
    /// </summary>
    public bool EstaEmAberto()
    {
        return Status == StatusChamado.Aberto || Status == StatusChamado.EmAtendimento;
    }

    /// <summary>
    /// Encerra o chamado registrando a solução aplicada.
    /// </summary>
    public void Encerrar(string solucao, StatusChamado statusFinal)
    {
        if (!EstaEmAberto())
        {
            throw new RegraDeNegocioException($"O chamado {Protocolo} já está encerrado.");
        }

        if (statusFinal != StatusChamado.Resolvido &&
            statusFinal != StatusChamado.Fechado &&
            statusFinal != StatusChamado.Cancelado)
        {
            throw new RegraDeNegocioException(
                "O encerramento deve utilizar os status Resolvido, Fechado ou Cancelado.");
        }

        Status = statusFinal;
        SolucaoAplicada = solucao;
        FechadoEm = DateTime.UtcNow;
        AtualizadoEm = DateTime.UtcNow;
    }
}
