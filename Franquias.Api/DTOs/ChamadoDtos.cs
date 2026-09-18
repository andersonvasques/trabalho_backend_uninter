using System.ComponentModel.DataAnnotations;
using Franquias.Api.Common;
using Franquias.Api.Models;

namespace Franquias.Api.DTOs;

/// <summary>
/// Dados para abertura de um chamado pela unidade franqueada.
/// </summary>
public sealed class AbrirChamadoDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Informe a unidade que está abrindo o chamado.")]
    public int UnidadeFranqueadaId { get; set; }

    [Required(ErrorMessage = "O título é obrigatório.")]
    [StringLength(150, MinimumLength = 5, ErrorMessage = "O título deve ter entre 5 e 150 caracteres.")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "A descrição é obrigatória.")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "A descrição deve ter entre 10 e 2000 caracteres.")]
    public string Descricao { get; set; } = string.Empty;

    [EnumDataType(typeof(CategoriaChamado), ErrorMessage = "Categoria de chamado inválida.")]
    public CategoriaChamado Categoria { get; set; } = CategoriaChamado.Outros;

    [EnumDataType(typeof(PrioridadeChamado), ErrorMessage = "Prioridade inválida.")]
    public PrioridadeChamado Prioridade { get; set; } = PrioridadeChamado.Media;
}

/// <summary>
/// Dados para registrar uma interação (resposta ou andamento) no chamado.
/// </summary>
public sealed class RegistrarInteracaoDto
{
    [Required(ErrorMessage = "A mensagem é obrigatória.")]
    [StringLength(2000, MinimumLength = 3, ErrorMessage = "A mensagem deve ter entre 3 e 2000 caracteres.")]
    public string Mensagem { get; set; } = string.Empty;

    /// <summary>
    /// Novo status do chamado. Quando não informado, o status é mantido.
    /// </summary>
    [EnumDataType(typeof(StatusChamado), ErrorMessage = "Status de chamado inválido.")]
    public StatusChamado? NovoStatus { get; set; }
}

/// <summary>
/// Dados para encerramento de um chamado.
/// </summary>
public sealed class EncerrarChamadoDto
{
    [Required(ErrorMessage = "Informe a solução aplicada.")]
    [StringLength(2000, MinimumLength = 5, ErrorMessage = "A solução deve ter entre 5 e 2000 caracteres.")]
    public string SolucaoAplicada { get; set; } = string.Empty;

    /// <summary>Status final: Resolvido, Fechado ou Cancelado.</summary>
    [EnumDataType(typeof(StatusChamado), ErrorMessage = "Status de chamado inválido.")]
    public StatusChamado StatusFinal { get; set; } = StatusChamado.Resolvido;
}

/// <summary>
/// Representação de um chamado devolvida pela API.
/// </summary>
public sealed class ChamadoDto
{
    public int Id { get; set; }

    public string Protocolo { get; set; } = string.Empty;

    public int UnidadeFranqueadaId { get; set; }

    public string? UnidadeNome { get; set; }

    public int UsuarioAberturaId { get; set; }

    public string? UsuarioAberturaNome { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;

    public CategoriaChamado Categoria { get; set; }

    public PrioridadeChamado Prioridade { get; set; }

    public StatusChamado Status { get; set; }

    public DateTime AbertoEm { get; set; }

    public DateTime? AtualizadoEm { get; set; }

    public DateTime? FechadoEm { get; set; }

    public string? SolucaoAplicada { get; set; }

    public IReadOnlyCollection<InteracaoDto> Interacoes { get; set; } = new List<InteracaoDto>();

    public static ChamadoDto DeModelo(ChamadoSuporte chamado)
    {
        var interacoes = new List<InteracaoDto>();

        foreach (ChamadoInteracao interacao in chamado.Interacoes)
        {
            interacoes.Add(InteracaoDto.DeModelo(interacao));
        }

        return new ChamadoDto
        {
            Id = chamado.Id,
            Protocolo = chamado.Protocolo,
            UnidadeFranqueadaId = chamado.UnidadeFranqueadaId,
            UnidadeNome = chamado.Unidade?.NomeFantasia,
            UsuarioAberturaId = chamado.UsuarioAberturaId,
            UsuarioAberturaNome = chamado.UsuarioAbertura?.Nome,
            Titulo = chamado.Titulo,
            Descricao = chamado.Descricao,
            Categoria = chamado.Categoria,
            Prioridade = chamado.Prioridade,
            Status = chamado.Status,
            AbertoEm = chamado.AbertoEm,
            AtualizadoEm = chamado.AtualizadoEm,
            FechadoEm = chamado.FechadoEm,
            SolucaoAplicada = chamado.SolucaoAplicada,
            Interacoes = interacoes
        };
    }
}

/// <summary>
/// Interação registrada em um chamado.
/// </summary>
public sealed class InteracaoDto
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public string? UsuarioNome { get; set; }

    public string Mensagem { get; set; } = string.Empty;

    public StatusChamado StatusRegistrado { get; set; }

    public DateTime RegistradaEm { get; set; }

    public static InteracaoDto DeModelo(ChamadoInteracao interacao)
    {
        return new InteracaoDto
        {
            Id = interacao.Id,
            UsuarioId = interacao.UsuarioId,
            UsuarioNome = interacao.Usuario?.Nome,
            Mensagem = interacao.Mensagem,
            StatusRegistrado = interacao.StatusRegistrado,
            RegistradaEm = interacao.RegistradaEm
        };
    }
}

/// <summary>
/// Filtros aceitos na listagem de chamados.
/// </summary>
public sealed class ConsultaChamadosDto : ParametrosDeConsulta
{
    public int? UnidadeFranqueadaId { get; set; }

    public StatusChamado? Status { get; set; }

    public PrioridadeChamado? Prioridade { get; set; }

    public CategoriaChamado? Categoria { get; set; }

    /// <summary>Quando verdadeiro, retorna apenas chamados em aberto ou em atendimento.</summary>
    public bool ApenasEmAberto { get; set; }

    /// <summary>Texto pesquisado no protocolo, título ou descrição.</summary>
    public string? Busca { get; set; }
}
