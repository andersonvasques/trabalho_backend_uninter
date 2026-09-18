using Franquias.Api.Common;
using Franquias.Api.DTOs;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

/// <summary>
/// Fornecedores homologados e a associação deles com os produtos da rede.
/// </summary>
[ApiController]
[Route("api/fornecedores")]
[Produces("application/json")]
[Authorize]
public sealed class FornecedoresController : ControllerBase
{
    private readonly IFornecedorService _fornecedorService;

    public FornecedoresController(IFornecedorService fornecedorService)
    {
        _fornecedorService = fornecedorService;
    }

    /// <summary>
    /// Lista os fornecedores com filtros e paginação.
    /// GET /api/fornecedores?busca=distribuidora&amp;ativo=true
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ResultadoPaginado<FornecedorDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResultadoPaginado<FornecedorDto>>> Listar(
        [FromQuery] ConsultaFornecedoresDto filtros,
        CancellationToken cancellationToken)
    {
        ResultadoPaginado<FornecedorDto> resultado =
            await _fornecedorService.ListarAsync(filtros, cancellationToken);

        return Ok(resultado);
    }

    /// <summary>
    /// Consulta um fornecedor pelo identificador.
    /// GET /api/fornecedores/1
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(FornecedorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FornecedorDto>> ObterPorId(
        int id,
        CancellationToken cancellationToken)
    {
        FornecedorDto fornecedor = await _fornecedorService.ObterPorIdAsync(id, cancellationToken);

        return Ok(fornecedor);
    }

    /// <summary>
    /// Cadastra um novo fornecedor.
    /// POST /api/fornecedores
    /// </summary>
    [HttpPost]
    [Authorize(Roles = Perfis.Administrador)]
    [ProducesResponseType(typeof(FornecedorDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<FornecedorDto>> Criar(
        [FromBody] FornecedorEntradaDto dados,
        CancellationToken cancellationToken)
    {
        FornecedorDto fornecedor = await _fornecedorService.CriarAsync(dados, cancellationToken);

        return CreatedAtAction(nameof(ObterPorId), new { id = fornecedor.Id }, fornecedor);
    }

    /// <summary>
    /// Atualiza um fornecedor.
    /// PUT /api/fornecedores/1
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = Perfis.Administrador)]
    [ProducesResponseType(typeof(FornecedorDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<FornecedorDto>> Atualizar(
        int id,
        [FromBody] FornecedorEntradaDto dados,
        CancellationToken cancellationToken)
    {
        FornecedorDto fornecedor = await _fornecedorService.AtualizarAsync(id, dados, cancellationToken);

        return Ok(fornecedor);
    }

    /// <summary>
    /// Reativa um fornecedor.
    /// PATCH /api/fornecedores/1/ativar
    /// </summary>
    [HttpPatch("{id:int}/ativar")]
    [Authorize(Roles = Perfis.Administrador)]
    [ProducesResponseType(typeof(FornecedorDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<FornecedorDto>> Ativar(
        int id,
        CancellationToken cancellationToken)
    {
        FornecedorDto fornecedor =
            await _fornecedorService.AlterarSituacaoAsync(id, true, cancellationToken);

        return Ok(fornecedor);
    }

    /// <summary>
    /// Inativa o fornecedor (exclusão lógica).
    /// DELETE /api/fornecedores/1
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = Perfis.Administrador)]
    [ProducesResponseType(typeof(FornecedorDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<FornecedorDto>> Inativar(
        int id,
        CancellationToken cancellationToken)
    {
        FornecedorDto fornecedor =
            await _fornecedorService.AlterarSituacaoAsync(id, false, cancellationToken);

        return Ok(fornecedor);
    }

    /// <summary>
    /// Associa um produto ao fornecedor informando custo e prazo de entrega.
    /// POST /api/fornecedores/1/produtos
    /// </summary>
    [HttpPost("{id:int}/produtos")]
    [Authorize(Roles = Perfis.Administrador)]
    [ProducesResponseType(typeof(FornecedorDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<FornecedorDto>> VincularProduto(
        int id,
        [FromBody] VincularProdutoDto dados,
        CancellationToken cancellationToken)
    {
        FornecedorDto fornecedor =
            await _fornecedorService.VincularProdutoAsync(id, dados, cancellationToken);

        return Ok(fornecedor);
    }

    /// <summary>
    /// Remove a associação entre o fornecedor e um produto.
    /// DELETE /api/fornecedores/1/produtos/5
    /// </summary>
    [HttpDelete("{id:int}/produtos/{produtoId:int}")]
    [Authorize(Roles = Perfis.Administrador)]
    [ProducesResponseType(typeof(FornecedorDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<FornecedorDto>> DesvincularProduto(
        int id,
        int produtoId,
        CancellationToken cancellationToken)
    {
        FornecedorDto fornecedor =
            await _fornecedorService.DesvincularProdutoAsync(id, produtoId, cancellationToken);

        return Ok(fornecedor);
    }
}
