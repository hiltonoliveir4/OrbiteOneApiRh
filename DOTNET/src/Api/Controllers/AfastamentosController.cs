using Application.DTOs.Afastamentos;
using Application.Exceptions;
using Application.Services;
using Application.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Api.Controllers;

[ApiController]
[Route("afastamentos")]
[Authorize]
public class AfastamentosController : ControllerBase
{
    private readonly AfastamentoService _service;

    public AfastamentosController(AfastamentoService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] AfastamentoCreateDto dto, CancellationToken cancellationToken)
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
        var items = CsvImportParser.ParseAfastamentos(body);
        var result = await _service.CriarEmLote(items, cancellationToken);
        return StatusCode(201, result);
    }

    [HttpPost("import/json")]
    public async Task<IActionResult> CriarEmLoteJson([FromBody] JsonElement body, CancellationToken cancellationToken)
    {
        if (body.ValueKind != JsonValueKind.Array)
        {
            throw new OrbiteOneException("Lista de afastamentos inválida");
        }

        var items = JsonSerializer.Deserialize<List<AfastamentoImportDto>>(body.GetRawText()) ?? new List<AfastamentoImportDto>();
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

    [HttpGet("matricula/{matricula}")]
    public async Task<IActionResult> ListarPorMatricula(string matricula, CancellationToken cancellationToken)
    {
        var itens = await _service.ListarPorMatricula(matricula, cancellationToken);
        return Ok(itens);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> BuscarPorId(int id, CancellationToken cancellationToken)
    {
        var item = await _service.BuscarPorId(id, cancellationToken);
        return Ok(item);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] AfastamentoUpdateDto dto, CancellationToken cancellationToken)
    {
        var item = await _service.Atualizar(id, dto, cancellationToken);
        return Ok(item);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Remover(int id, CancellationToken cancellationToken)
    {
        await _service.Remover(id, cancellationToken);
        return NoContent();
    }
}
