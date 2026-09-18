using Franquias.Api.Common;
using Franquias.Api.DTOs;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

/// <summary>
/// Chamados de suporte abertos pelas unidades para a franqueadora.
/// </summary>
[ApiController]
[Route("api/chamados")]
[Produces("application/json")]
[Authorize]
public sealed class ChamadosController : ControllerBase
{
    private readonly IChamadoService _chamadoService;

    public ChamadosController(IChamadoService chamadoService)
    {
        _chamadoService = chamadoService;
    }

    /// <summary>
    /// Lista os chamados por unidade, status, prioridade e categoria.
    /// GET /api/chamados?apenasEmAberto=true&amp;prioridade=Alta
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ResultadoPaginado<ChamadoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResultadoPaginado<ChamadoDto>>> Listar(
        [FromQuery] ConsultaChamadosDto filtros,
        CancellationToken cancellationToken)
    {
        ResultadoPaginado<ChamadoDto> resultado =
            await _chamadoService.ListarAsync(filtros, cancellationToken);

        return Ok(resultado);
    }

    /// <summary>
    /// Consulta um chamado e o histórico de interações.
    /// GET /api/chamados/1
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ChamadoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ChamadoDto>> ObterPorId(
        int id,
        CancellationToken cancellationToken)
    {
        ChamadoDto chamado = await _chamadoService.ObterPorIdAsync(id, cancellationToken);

        return Ok(chamado);
    }

    /// <summary>
    /// Abre um chamado para a franqueadora.
    /// POST /api/chamados
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ChamadoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ChamadoDto>> Abrir(
        [FromBody] AbrirChamadoDto dados,
        CancellationToken cancellationToken)
    {
        ChamadoDto chamado = await _chamadoService.AbrirAsync(dados, cancellationToken);

        return CreatedAtAction(nameof(ObterPorId), new { id = chamado.Id }, chamado);
    }

    /// <summary>
    /// Registra uma interação (resposta ou andamento) no chamado.
    /// POST /api/chamados/1/interacoes
    /// </summary>
    [HttpPost("{id:int}/interacoes")]
    [ProducesResponseType(typeof(ChamadoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ChamadoDto>> RegistrarInteracao(
        int id,
        [FromBody] RegistrarInteracaoDto dados,
        CancellationToken cancellationToken)
    {
        ChamadoDto chamado = await _chamadoService.RegistrarInteracaoAsync(id, dados, cancellationToken);

        return Ok(chamado);
    }

    /// <summary>
    /// Encerra o chamado registrando a solução aplicada.
    /// PUT /api/chamados/1/encerramento
    /// </summary>
    [HttpPut("{id:int}/encerramento")]
    [Authorize(Roles = Perfis.AdministradorOuGestor)]
    [ProducesResponseType(typeof(ChamadoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ChamadoDto>> Encerrar(
        int id,
        [FromBody] EncerrarChamadoDto dados,
        CancellationToken cancellationToken)
    {
        ChamadoDto chamado = await _chamadoService.EncerrarAsync(id, dados, cancellationToken);

        return Ok(chamado);
    }
}
