using System.Security.Cryptography;

namespace Franquias.Api.Security;

/// <summary>
/// Responsável por transformar a senha informada pelo usuário em um hash seguro.
///
/// É utilizado o algoritmo PBKDF2 com SHA-256 e um salt aleatório por usuário,
/// de modo que a senha original nunca é armazenada no banco de dados.
/// </summary>
public static class SegurancaDeSenha
{
    private const int TamanhoDoSaltEmBytes = 16;
    private const int TamanhoDoHashEmBytes = 32;
    private const int QuantidadeDeIteracoes = 100_000;

    /// <summary>
    /// Gera o hash e o salt de uma nova senha.
    /// </summary>
    public static (string Hash, string Salt) GerarHash(string senha)
    {
        if (string.IsNullOrWhiteSpace(senha))
        {
            throw new ArgumentException("A senha não pode ser vazia.", nameof(senha));
        }

        byte[] salt = RandomNumberGenerator.GetBytes(TamanhoDoSaltEmBytes);

        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password: senha,
            salt: salt,
            iterations: QuantidadeDeIteracoes,
            hashAlgorithm: HashAlgorithmName.SHA256,
            outputLength: TamanhoDoHashEmBytes);

        return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
    }

    /// <summary>
    /// Confere se a senha informada corresponde ao hash armazenado.
    /// A comparação é feita em tempo fixo para evitar ataques de temporização.
    /// </summary>
    public static bool Conferir(string senha, string hashArmazenado, string saltArmazenado)
    {
        if (string.IsNullOrWhiteSpace(senha) ||
            string.IsNullOrWhiteSpace(hashArmazenado) ||
            string.IsNullOrWhiteSpace(saltArmazenado))
        {
            return false;
        }

        byte[] salt;
        byte[] hashEsperado;

        try
        {
            salt = Convert.FromBase64String(saltArmazenado);
            hashEsperado = Convert.FromBase64String(hashArmazenado);
        }
        catch (FormatException)
        {
            return false;
        }

        byte[] hashInformado = Rfc2898DeriveBytes.Pbkdf2(
            password: senha,
            salt: salt,
            iterations: QuantidadeDeIteracoes,
            hashAlgorithm: HashAlgorithmName.SHA256,
            outputLength: hashEsperado.Length);

        return CryptographicOperations.FixedTimeEquals(hashInformado, hashEsperado);
    }
}
