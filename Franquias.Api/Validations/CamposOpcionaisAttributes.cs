using System.ComponentModel.DataAnnotations;

namespace Franquias.Api.Validations;

/// <summary>
/// Valida um e-mail somente quando algum valor é informado.
///
/// O atributo padrão [EmailAddress] considera a string vazia inválida, o que
/// tornaria obrigatórios campos que o sistema trata como opcionais.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class EmailOpcionalAttribute : ValidationAttribute
{
    private static readonly EmailAddressAttribute Validador = new();

    public EmailOpcionalAttribute()
        : base("Informe um e-mail válido ou deixe o campo em branco.")
    {
    }

    public override bool IsValid(object? value)
    {
        string? texto = value as string;

        if (string.IsNullOrWhiteSpace(texto))
        {
            return true;
        }

        return Validador.IsValid(texto);
    }
}

/// <summary>
/// Valida a unidade federativa (2 letras) somente quando informada.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class UfOpcionalAttribute : ValidationAttribute
{
    public UfOpcionalAttribute()
        : base("A UF deve ter 2 letras (ex.: PR) ou ficar em branco.")
    {
    }

    public override bool IsValid(object? value)
    {
        string? texto = value as string;

        if (string.IsNullOrWhiteSpace(texto))
        {
            return true;
        }

        texto = texto.Trim();

        return texto.Length == 2 && char.IsLetter(texto[0]) && char.IsLetter(texto[1]);
    }
}

/// <summary>
/// Valida o CEP (8 dígitos, sem pontuação) somente quando informado.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class CepOpcionalAttribute : ValidationAttribute
{
    public CepOpcionalAttribute()
        : base("O CEP deve conter 8 dígitos, sem pontuação, ou ficar em branco.")
    {
    }

    public override bool IsValid(object? value)
    {
        string? texto = value as string;

        if (string.IsNullOrWhiteSpace(texto))
        {
            return true;
        }

        texto = texto.Trim();

        if (texto.Length != 8)
        {
            return false;
        }

        foreach (char caractere in texto)
        {
            if (!char.IsDigit(caractere))
            {
                return false;
            }
        }

        return true;
    }
}
