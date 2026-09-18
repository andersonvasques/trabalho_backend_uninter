using Franquias.Api.Common;
using Franquias.Api.DTOs;
using Franquias.Api.Models;
using Franquias.Api.Repositories;
using Franquias.Api.Security;
using Franquias.Api.Validations;

namespace Franquias.Api.Services;

/// <summary>
/// Implementação das regras de cadastro das unidades franqueadas.
/// </summary>
public sealed class UnidadeService : IUnidadeService
{
    private readonly IUnidadeRepositorio _unidadeRepositorio;
    private readonly IRepositorioBase<Franqueadora> _franqueadoraRepositorio;
    private readonly IRepositorioBase<Franqueado> _franqueadoRepositorio;
    private readonly IUsuarioContexto _usuarioContexto;

    public UnidadeService(
        IUnidadeRepositorio unidadeRepositorio,
        IRepositorioBase<Franqueadora> franqueadoraRepositorio,
        IRepositorioBase<Franqueado> franqueadoRepositorio,
        IUsuarioContexto usuarioContexto)
    {
        _unidadeRepositorio = unidadeRepositorio;
        _franqueadoraRepositorio = franqueadoraRepositorio;
        _franqueadoRepositorio = franqueadoRepositorio;
        _usuarioContexto = usuarioContexto;
    }

    public async Task<ResultadoPaginado<UnidadeDto>> ListarAsync(
        ConsultaUnidadesDto filtros,
        CancellationToken cancellationToken)
    {
        // Gestores e operadores enxergam somente a própria unidade.
        filtros.UnidadeFranqueadaId = _usuarioContexto.ResolverFiltroDeUnidade(filtros.UnidadeFranqueadaId);

        ResultadoPaginado<UnidadeFranqueada> pagina =
            await _unidadeRepositorio.ListarPaginadoAsync(filtros, cancellationToken);

        var itens = new List<UnidadeDto>();

        foreach (UnidadeFranqueada unidade in pagina.Itens)
        {
            itens.Add(UnidadeDto.DeModelo(unidade));
        }

        return new ResultadoPaginado<UnidadeDto>(
            itens,
            pagina.TotalItens,
            pagina.Pagina,
            pagina.TamanhoPagina);
    }

    public async Task<UnidadeDto> ObterPorIdAsync(int id, CancellationToken cancellationToken)
    {
        UnidadeFranqueada unidade = await ObterUnidadeAsync(id, cancellationToken);

        return UnidadeDto.DeModelo(unidade);
    }

    public async Task<UnidadeDto> CriarAsync(
        CriarUnidadeDto dados,
        CancellationToken cancellationToken)
    {
        string cnpj = ValidadorDeDocumentos.SomenteDigitos(dados.Cnpj);

        // Regra de negócio: não é permitido cadastrar duas unidades com o mesmo CNPJ.
        bool cnpjEmUso = await _unidadeRepositorio.CnpjJaCadastradoAsync(cnpj, null, cancellationToken);

        if (cnpjEmUso)
        {
            throw new ConflitoException($"Já existe uma unidade cadastrada com o CNPJ {cnpj}.");
        }

        bool codigoEmUso = await _unidadeRepositorio.CodigoJaCadastradoAsync(
            dados.Codigo,
            null,
            cancellationToken);

        if (codigoEmUso)
        {
            throw new ConflitoException($"Já existe uma unidade com o código {dados.Codigo}.");
        }

        await GarantirFranqueadoraExistenteAsync(dados.FranqueadoraId, cancellationToken);
        await GarantirFranqueadoExistenteAsync(dados.FranqueadoId, cancellationToken);

        var unidade = new UnidadeFranqueada
        {
            Codigo = dados.Codigo.Trim().ToUpper(),
            FranqueadoraId = dados.FranqueadoraId,
            FranqueadoId = dados.FranqueadoId,
            RazaoSocial = dados.RazaoSocial.Trim(),
            NomeFantasia = dados.NomeFantasia.Trim(),
            Cnpj = cnpj,
            Email = dados.Email.Trim().ToLower(),
            Telefone = dados.Telefone.Trim(),
            Logradouro = dados.Logradouro.Trim(),
            Numero = dados.Numero.Trim(),
            Bairro = dados.Bairro.Trim(),
            Cidade = dados.Cidade.Trim(),
            Uf = dados.Uf.Trim().ToUpper(),
            Cep = ValidadorDeDocumentos.SomenteDigitos(dados.Cep),
            ResponsavelNome = dados.ResponsavelNome.Trim(),
            ResponsavelTelefone = dados.ResponsavelTelefone.Trim(),
            DataInicioContrato = dados.DataInicioContrato,
            PercentualRoyalty = dados.PercentualRoyalty,
            Situacao = dados.Situacao,
            CriadaEm = DateTime.UtcNow
        };

        await _unidadeRepositorio.AdicionarAsync(unidade, cancellationToken);
        await _unidadeRepositorio.SalvarAlteracoesAsync(cancellationToken);

        UnidadeFranqueada? criada = await _unidadeRepositorio.ObterCompletaAsync(
            unidade.Id,
            cancellationToken);

        return UnidadeDto.DeModelo(criada ?? unidade);
    }

    public async Task<UnidadeDto> AtualizarAsync(
        int id,
        AtualizarUnidadeDto dados,
        CancellationToken cancellationToken)
    {
        UnidadeFranqueada unidade = await ObterUnidadeAsync(id, cancellationToken);

        await GarantirFranqueadoExistenteAsync(dados.FranqueadoId, cancellationToken);

        unidade.RazaoSocial = dados.RazaoSocial.Trim();
        unidade.NomeFantasia = dados.NomeFantasia.Trim();
        unidade.FranqueadoId = dados.FranqueadoId;
        unidade.Email = dados.Email.Trim().ToLower();
        unidade.Telefone = dados.Telefone.Trim();
        unidade.Logradouro = dados.Logradouro.Trim();
        unidade.Numero = dados.Numero.Trim();
        unidade.Bairro = dados.Bairro.Trim();
        unidade.Cidade = dados.Cidade.Trim();
        unidade.Uf = dados.Uf.Trim().ToUpper();
        unidade.Cep = ValidadorDeDocumentos.SomenteDigitos(dados.Cep);
        unidade.ResponsavelNome = dados.ResponsavelNome.Trim();
        unidade.ResponsavelTelefone = dados.ResponsavelTelefone.Trim();
        unidade.PercentualRoyalty = dados.PercentualRoyalty;
        unidade.Situacao = dados.Situacao;
        unidade.AtualizadaEm = DateTime.UtcNow;

        if (dados.Situacao == SituacaoUnidade.Inativa)
        {
            unidade.DataEncerramentoContrato ??= DateTime.UtcNow;
        }
        else
        {
            unidade.DataEncerramentoContrato = null;
        }

        await _unidadeRepositorio.SalvarAlteracoesAsync(cancellationToken);

        return UnidadeDto.DeModelo(unidade);
    }

    public async Task<UnidadeDto> InativarAsync(int id, CancellationToken cancellationToken)
    {
        UnidadeFranqueada unidade = await ObterUnidadeAsync(id, cancellationToken);

        if (unidade.Situacao == SituacaoUnidade.Inativa)
        {
            throw new RegraDeNegocioException($"A unidade {unidade.Codigo} já está inativa.");
        }

        // Exclusão lógica: o histórico da unidade é preservado.
        unidade.Inativar();

        await _unidadeRepositorio.SalvarAlteracoesAsync(cancellationToken);

        return UnidadeDto.DeModelo(unidade);
    }

    public async Task<UnidadeDto> ReativarAsync(int id, CancellationToken cancellationToken)
    {
        UnidadeFranqueada unidade = await ObterUnidadeAsync(id, cancellationToken);

        if (unidade.Situacao == SituacaoUnidade.Ativa)
        {
            throw new RegraDeNegocioException($"A unidade {unidade.Codigo} já está ativa.");
        }

        unidade.Situacao = SituacaoUnidade.Ativa;
        unidade.DataEncerramentoContrato = null;
        unidade.AtualizadaEm = DateTime.UtcNow;

        await _unidadeRepositorio.SalvarAlteracoesAsync(cancellationToken);

        return UnidadeDto.DeModelo(unidade);
    }

    private async Task<UnidadeFranqueada> ObterUnidadeAsync(int id, CancellationToken cancellationToken)
    {
        UnidadeFranqueada? unidade = await _unidadeRepositorio.ObterCompletaAsync(id, cancellationToken);

        if (unidade is null)
        {
            throw new NaoEncontradoException($"A unidade {id} não foi encontrada.");
        }

        // Gestores e operadores só acessam a própria unidade.
        _usuarioContexto.GarantirAcessoAUnidade(unidade.Id);

        return unidade;
    }

    private async Task GarantirFranqueadoraExistenteAsync(int id, CancellationToken cancellationToken)
    {
        Franqueadora? franqueadora = await _franqueadoraRepositorio.ObterPorIdAsync(id, cancellationToken);

        if (franqueadora is null)
        {
            throw new NaoEncontradoException($"A franqueadora {id} não foi encontrada.");
        }
    }

    private async Task GarantirFranqueadoExistenteAsync(int id, CancellationToken cancellationToken)
    {
        Franqueado? franqueado = await _franqueadoRepositorio.ObterPorIdAsync(id, cancellationToken);

        if (franqueado is null)
        {
            throw new NaoEncontradoException($"O franqueado {id} não foi encontrado.");
        }

        if (!franqueado.Ativo)
        {
            throw new RegraDeNegocioException(
                "Não é possível vincular a unidade a um franqueado inativo.");
        }
    }
}
