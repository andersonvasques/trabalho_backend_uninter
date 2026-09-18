using Franquias.Api.Common;
using Franquias.Api.DTOs;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

/// <summary>
/// Catálogo de produtos e serviços padronizados pela franqueadora.
/// </summary>
[ApiController]
[Route("api/produtos")]
[Produces("application/json")]
[Authorize]
public sealed class ProdutosController : ControllerBase
{
    private readonly IProdutoService _produtoService;

    public ProdutosController(IProdutoService produtoService)
    {
        _produtoService = produtoService;
    }

    /// <summary>
    /// Lista os itens do catálogo com filtros, ordenação e paginação.
    /// GET /api/produtos?busca=cafe&amp;categoriaId=1&amp;tipo=Produto&amp;ativo=true
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ResultadoPaginado<ProdutoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResultadoPaginado<ProdutoDto>>> Listar(
        [FromQuery] ConsultaProdutosDto filtros,
        CancellationToken cancellationToken)
    {
        ResultadoPaginado<ProdutoDto> resultado =
            await _produtoService.ListarAsync(filtros, cancellationToken);

        return Ok(resultado);
    }

    /// <summary>
    /// Consulta um item do catálogo pelo identificador.
    /// GET /api/produtos/1
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProdutoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProdutoDto>> ObterPorId(
        int id,
        CancellationToken cancellationToken)
    {
        ProdutoDto produto = await _produtoService.ObterPorIdAsync(id, cancellationToken);

        return Ok(produto);
    }

    /// <summary>
    /// Cadastra um novo produto ou serviço.
    /// POST /api/produtos
    /// </summary>
    [HttpPost]
    [Authorize(Roles = Perfis.Administrador)]
    [ProducesResponseType(typeof(ProdutoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProdutoDto>> Criar(
        [FromBody] CriarProdutoDto dados,
        CancellationToken cancellationToken)
    {
        ProdutoDto produto = await _produtoService.CriarAsync(dados, cancellationToken);

        return CreatedAtAction(nameof(ObterPorId), new { id = produto.Id }, produto);
    }

    /// <summary>
    /// Atualiza um produto ou serviço.
    /// PUT /api/produtos/1
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = Perfis.Administrador)]
    [ProducesResponseType(typeof(ProdutoDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ProdutoDto>> Atualizar(
        int id,
        [FromBody] AtualizarProdutoDto dados,
        CancellationToken cancellationToken)
    {
        ProdutoDto produto = await _produtoService.AtualizarAsync(id, dados, cancellationToken);

        return Ok(produto);
    }

    /// <summary>
    /// Reativa um item do catálogo.
    /// PATCH /api/produtos/1/ativar
    /// </summary>
    [HttpPatch("{id:int}/ativar")]
    [Authorize(Roles = Perfis.Administrador)]
    [ProducesResponseType(typeof(ProdutoDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ProdutoDto>> Ativar(
        int id,
        CancellationToken cancellationToken)
    {
        ProdutoDto produto = await _produtoService.AlterarSituacaoAsync(id, true, cancellationToken);

        return Ok(produto);
    }

    /// <summary>
    /// Inativa o item do catálogo (exclusão lógica).
    /// DELETE /api/produtos/1
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = Perfis.Administrador)]
    [ProducesResponseType(typeof(ProdutoDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ProdutoDto>> Inativar(
        int id,
        CancellationToken cancellationToken)
    {
        ProdutoDto produto = await _produtoService.AlterarSituacaoAsync(id, false, cancellationToken);

        return Ok(produto);
    }
}
