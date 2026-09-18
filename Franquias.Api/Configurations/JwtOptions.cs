namespace Franquias.Api.Configurations;

/// <summary>
/// Configurações do token JWT lidas da seção "Jwt" do appsettings.json.
/// </summary>
public sealed class JwtOptions
{
    /// <summary>Nome da seção no arquivo de configuração.</summary>
    public const string Secao = "Jwt";

    /// <summary>Chave simétrica utilizada para assinar o token.</summary>
    public string Chave { get; set; } = string.Empty;

    /// <summary>Emissor do token.</summary>
    public string Emissor { get; set; } = string.Empty;

    /// <summary>Público-alvo do token.</summary>
    public string Audiencia { get; set; } = string.Empty;

    /// <summary>Tempo de validade do token em minutos.</summary>
    public int ExpiracaoEmMinutos { get; set; } = 480;
}
