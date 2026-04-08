using Application.DTOs.Afastamentos;
using Application.DTOs.Common;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class AfastamentoService
{
    private readonly IAfastamentoRepository _repository;

    public AfastamentoService(IAfastamentoRepository repository)
    {
        _repository = repository;
    }

    public async Task<AfastamentoDto> Criar(AfastamentoCreateDto dto, CancellationToken cancellationToken = default)
    {
        var entity = MapCreate(dto);
        var criado = await _repository.Criar(entity, cancellationToken);
        return MapDto(criado);
    }

    public async Task<ImportResultDto> CriarEmLote(IEnumerable<(int linha, AfastamentoImportDto data)> itens, CancellationToken cancellationToken = default)
    {
        var linhasComErro = new List<ImportErrorLineDto>();
        var criados = 0;
        var atualizados = 0;

        foreach (var (linha, data) in itens)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(data.Matricula))
                {
                    throw new OrbiteOneException("Matrícula não informada");
                }
                if (!data.EmpregadoId.HasValue)
                {
                    throw new OrbiteOneException("Empregado id não informado");
                }
                if (string.IsNullOrWhiteSpace(data.Controle))
                {
                    throw new OrbiteOneException("Controle não informado");
                }

                var existentes = await _repository.ListarPorMatricula(data.Matricula, cancellationToken);
                if (existentes.Count > 0)
                {
                    var existente = existentes[0];
                    ApplyPatch(existente, data);
                    await _repository.Atualizar(existente, cancellationToken);
                    atualizados += 1;
                }
                else
                {
                    var novo = MapImportToCreate(data);
                    var criado = await _repository.Criar(novo, cancellationToken);
                    _ = criado;
                    criados += 1;
                }
            }
            catch (Exception ex)
            {
                linhasComErro.Add(new ImportErrorLineDto
                {
                    Linha = linha,
                    Erro = ex is OrbiteOneException ? ex.Message : "Erro ao processar linha",
                });
            }
        }

        var total = itens.Count();
        var sucesso = criados + atualizados;

        return new ImportResultDto
        {
            Total = total,
            Criados = criados,
            Atualizados = atualizados,
            Sucesso = sucesso,
            Erros = linhasComErro.Count,
            LinhasComErro = linhasComErro,
        };
    }

    public async Task<List<AfastamentoDto>> Listar(DateTime? dataMovimento, CancellationToken cancellationToken = default)
    {
        var itens = dataMovimento.HasValue
            ? await _repository.ListarPorDataMovimento(dataMovimento.Value, cancellationToken)
            : await _repository.Listar(cancellationToken);
        return itens.Select(MapDto).ToList();
    }

    public async Task<AfastamentoDto> BuscarPorId(int id, CancellationToken cancellationToken = default)
    {
        var item = await _repository.BuscarPorId(id, cancellationToken);
        if (item == null)
        {
            throw new OrbiteOneException("Afastamento não encontrado", 404);
        }
        return MapDto(item);
    }

    public async Task<List<AfastamentoDto>> ListarPorMatricula(string matricula, CancellationToken cancellationToken = default)
    {
        var itens = await _repository.ListarPorMatricula(matricula, cancellationToken);
        return itens.Select(MapDto).ToList();
    }

    public async Task<AfastamentoDto> Atualizar(int id, AfastamentoUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var item = await _repository.BuscarPorId(id, cancellationToken);
        if (item == null)
        {
            throw new OrbiteOneException("Afastamento não encontrado", 404);
        }

        ApplyPatch(item, dto);
        var atualizado = await _repository.Atualizar(item, cancellationToken);
        return MapDto(atualizado);
    }

    public async Task Remover(int id, CancellationToken cancellationToken = default)
    {
        var item = await _repository.BuscarPorId(id, cancellationToken);
        if (item == null)
        {
            throw new OrbiteOneException("Afastamento não encontrado", 404);
        }
        await _repository.Remover(item, cancellationToken);
    }

    private static Afastamento MapCreate(AfastamentoCreateDto dto)
    {
        return new Afastamento
        {
            Matricula = dto.Matricula,
            Descricao = dto.Descricao,
            DataInicio = dto.DataInicio,
            DataFinal = dto.DataFinal,
            CnpjUnidade = dto.CnpjUnidade,
            CodigoSituacao = dto.CodigoSituacao,
            DataMovimento = DateTime.Now,
            EmpregadoId = dto.EmpregadoId,
            FkPeriodo = null,
            Controle = dto.Controle,
        };
    }

    private static Afastamento MapImportToCreate(AfastamentoImportDto dto)
    {
        return new Afastamento
        {
            Matricula = dto.Matricula,
            Descricao = dto.Descricao,
            DataInicio = dto.DataInicio,
            DataFinal = dto.DataFinal,
            CnpjUnidade = dto.CnpjUnidade,
            CodigoSituacao = dto.CodigoSituacao,
            DataMovimento = DateTime.Now,
            EmpregadoId = dto.EmpregadoId ?? default,
            FkPeriodo = dto.Periodo,
            Controle = dto.Controle ?? string.Empty,
        };
    }

    private static void ApplyPatch(Afastamento afastamento, AfastamentoUpdateDto dto)
    {
        if (dto.Matricula != null) afastamento.Matricula = dto.Matricula;
        if (dto.Descricao != null) afastamento.Descricao = dto.Descricao;
        if (dto.DataInicio.HasValue) afastamento.DataInicio = dto.DataInicio.Value;
        if (dto.DataFinal.HasValue) afastamento.DataFinal = dto.DataFinal.Value;
        if (dto.CnpjUnidade != null) afastamento.CnpjUnidade = dto.CnpjUnidade;
        if (dto.CodigoSituacao != null) afastamento.CodigoSituacao = dto.CodigoSituacao;
        if (dto.EmpregadoId.HasValue) afastamento.EmpregadoId = dto.EmpregadoId.Value;
        if (!string.IsNullOrWhiteSpace(dto.Controle)) afastamento.Controle = dto.Controle;
        afastamento.DataMovimento = DateTime.Now;
    }

    private static void ApplyPatch(Afastamento afastamento, AfastamentoImportDto dto)
    {
        if (!string.IsNullOrWhiteSpace(dto.Matricula)) afastamento.Matricula = dto.Matricula;
        if (!string.IsNullOrWhiteSpace(dto.Descricao)) afastamento.Descricao = dto.Descricao;
        afastamento.DataInicio = dto.DataInicio;
        if (dto.DataFinal.HasValue) afastamento.DataFinal = dto.DataFinal.Value;
        if (dto.CnpjUnidade != null) afastamento.CnpjUnidade = dto.CnpjUnidade;
        if (dto.CodigoSituacao != null) afastamento.CodigoSituacao = dto.CodigoSituacao;
        if (dto.EmpregadoId.HasValue) afastamento.EmpregadoId = dto.EmpregadoId.Value;
        if (dto.Periodo.HasValue) afastamento.FkPeriodo = dto.Periodo.Value;
        if (!string.IsNullOrWhiteSpace(dto.Controle)) afastamento.Controle = dto.Controle;
        afastamento.DataMovimento = DateTime.Now;
    }

    private static AfastamentoDto MapDto(Afastamento afastamento)
    {
        return new AfastamentoDto
        {
            Id = afastamento.Id,
            Matricula = afastamento.Matricula,
            Descricao = afastamento.Descricao,
            DataInicio = afastamento.DataInicio,
            DataFinal = afastamento.DataFinal,
            CnpjUnidade = afastamento.CnpjUnidade,
            CodigoSituacao = afastamento.CodigoSituacao,
            DataMovimento = afastamento.DataMovimento,
        };
    }
}
