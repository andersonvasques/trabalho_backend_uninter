namespace Franquias.Api.Models;

/// <summary>
/// Cada mensagem ou mudança de situação registrada em um chamado.
/// Mantém o histórico da comunicação entre unidade e franqueadora.
/// </summary>
public sealed class ChamadoInteracao
{
    /// <summary>Identificador único da interação.</summary>
    public int Id { get; set; }

    /// <summary>Chamado ao qual a interação pertence.</summary>
    public int ChamadoSuporteId { get; set; }

    /// <summary>Dados do chamado.</summary>
    public ChamadoSuporte Chamado { get; set; } = null!;

    /// <summary>Usuário autor da interação.</summary>
    public int UsuarioId { get; set; }

    /// <summary>Dados do usuário.</summary>
    public Usuario Usuario { get; set; } = null!;

    /// <summary>Mensagem registrada.</summary>
    public string Mensagem { get; set; } = string.Empty;

    /// <summary>Situação do chamado após a interação.</summary>
    public StatusChamado StatusRegistrado { get; set; }

    /// <summary>Data e hora da interação.</summary>
    public DateTime RegistradaEm { get; set; }
}
