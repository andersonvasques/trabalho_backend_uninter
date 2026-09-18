using System.ComponentModel.DataAnnotations;

namespace Franquias.Api.Validations;

/// <summary>
/// Valida um CNPJ informado somente com dígitos (14 caracteres),
/// conferindo os dois dígitos verificadores.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class CnpjAttribute : ValidationAttribute
{
    public CnpjAttribute()
        : base("O CNPJ informado é inválido. Informe 14 dígitos, sem pontuação.")
    {
    }

    public override bool IsValid(object? value)
    {
        // Campos nulos são responsabilidade do atributo [Required].
        if (value is null)
        {
            return true;
        }

        string cnpj = value.ToString() ?? string.Empty;

        return ValidadorDeDocumentos.CnpjEhValido(cnpj);
    }
}

/// <summary>
/// Valida um CPF informado somente com dígitos (11 caracteres),
/// conferindo os dois dígitos verificadores.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class CpfAttribute : ValidationAttribute
{
    public CpfAttribute()
        : base("O CPF informado é inválido. Informe 11 dígitos, sem pontuação.")
    {
    }

    public override bool IsValid(object? value)
    {
        if (value is null)
        {
            return true;
        }

        string cpf = value.ToString() ?? string.Empty;

        return ValidadorDeDocumentos.CpfEhValido(cpf);
    }
}

/// <summary>
/// Regras de validação de CPF e CNPJ utilizadas pelos atributos acima
/// e também pelos serviços da aplicação.
/// </summary>
public static class ValidadorDeDocumentos
{
    /// <summary>
    /// Remove qualquer caractere que não seja dígito.
    /// </summary>
    public static string SomenteDigitos(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return string.Empty;
        }

        var digitos = new System.Text.StringBuilder(valor.Length);

        foreach (char caractere in valor)
        {
            if (char.IsDigit(caractere))
            {
                digitos.Append(caractere);
            }
        }

        return digitos.ToString();
    }

    /// <summary>
    /// Confere os dígitos verificadores de um CNPJ.
    /// </summary>
    public static bool CnpjEhValido(string? valor)
    {
        string cnpj = SomenteDigitos(valor);

        if (cnpj.Length != 14)
        {
            return false;
        }

        if (TodosOsDigitosSaoIguais(cnpj))
        {
            return false;
        }

        int[] pesosPrimeiroDigito = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] pesosSegundoDigito = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        int primeiroDigito = CalcularDigito(cnpj, pesosPrimeiroDigito);
        int segundoDigito = CalcularDigito(cnpj, pesosSegundoDigito);

        return cnpj[12] - '0' == primeiroDigito && cnpj[13] - '0' == segundoDigito;
    }

    /// <summary>
    /// Confere os dígitos verificadores de um CPF.
    /// </summary>
    public static bool CpfEhValido(string? valor)
    {
        string cpf = SomenteDigitos(valor);

        if (cpf.Length != 11)
        {
            return false;
        }

        if (TodosOsDigitosSaoIguais(cpf))
        {
            return false;
        }

        int[] pesosPrimeiroDigito = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] pesosSegundoDigito = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        int primeiroDigito = CalcularDigito(cpf, pesosPrimeiroDigito);
        int segundoDigito = CalcularDigito(cpf, pesosSegundoDigito);

        return cpf[9] - '0' == primeiroDigito && cpf[10] - '0' == segundoDigito;
    }

    private static int CalcularDigito(string documento, int[] pesos)
    {
        int soma = 0;

        for (int indice = 0; indice < pesos.Length; indice++)
        {
            soma += (documento[indice] - '0') * pesos[indice];
        }

        int resto = soma % 11;

        return resto < 2 ? 0 : 11 - resto;
    }

    private static bool TodosOsDigitosSaoIguais(string documento)
    {
        char primeiro = documento[0];

        foreach (char caractere in documento)
        {
            if (caractere != primeiro)
            {
                return false;
            }
        }

        return true;
    }
}
