using Application.DTOs.Colaboradores;
using Application.Exceptions;
using Application.Services;
using Application.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Api.Controllers;

[ApiController]
[Route("colaboradores")]
[Authorize]
public class ColaboradoresController : ControllerBase
{
    private readonly ColaboradorService _service;

    public ColaboradoresController(ColaboradorService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] ColaboradorCreateDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            throw new OrbiteOneException("Payload inválido");
        }

        var result = await _service.Criar(dto, cancellationToken);
        return StatusCode(201, result);
    }

    [HttpPost("import/csv")]
    public async Task<IActionResult> CriarEmLoteCsv(CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(Request.Body);
        var body = await reader.ReadToEndAsync(cancellationToken);
        var items = CsvImportParser.ParseColaboradores(body);
        var result = await _service.CriarEmLote(items, cancellationToken);
        return StatusCode(201, result);
    }

    [HttpPost("import/json")]
    public async Task<IActionResult> CriarEmLoteJson([FromBody] JsonElement body, CancellationToken cancellationToken)
    {
        if (body.ValueKind != JsonValueKind.Array)
        {
            throw new OrbiteOneException("Lista de colaboradores inválida");
        }

        var items = JsonSerializer.Deserialize<List<ColaboradorImportDto>>(body.GetRawText()) ?? new List<ColaboradorImportDto>();
        var mapped = items.Select((item, index) => (linha: index + 1, data: item)).ToList();
        var result = await _service.CriarEmLote(mapped, cancellationToken);
        return StatusCode(201, result);
    }

    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken cancellationToken)
    {
        var itens = await _service.Listar(cancellationToken);
        return Ok(itens);
    }

    [HttpGet("{matricula}")]
    public async Task<IActionResult> Buscar(string matricula, CancellationToken cancellationToken)
    {
        var item = await _service.BuscarPorMatricula(matricula, cancellationToken);
        return Ok(item);
    }

    [HttpPut("{matricula}")]
    public async Task<IActionResult> Atualizar(string matricula, [FromBody] ColaboradorUpdateDto dto, CancellationToken cancellationToken)
    {
        var item = await _service.Atualizar(matricula, dto, cancellationToken);
        return Ok(item);
    }

    [HttpDelete("{matricula}")]
    public async Task<IActionResult> Remover(string matricula, CancellationToken cancellationToken)
    {
        await _service.Remover(matricula, cancellationToken);
        return NoContent();
    }
}
