using Application.DTOs.Colaboradores;
using Application.DTOs.Common;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class ColaboradorService
{
    private readonly IColaboradorRepository _repository;

    public ColaboradorService(IColaboradorRepository repository)
    {
        _repository = repository;
    }

    public async Task<ColaboradorDto> Criar(ColaboradorCreateDto dto, CancellationToken cancellationToken = default)
    {
        var existente = await _repository.BuscarPorMatricula(dto.Matricula, cancellationToken);
        if (existente != null)
        {
            throw new OrbiteOneException("Colaborador já cadastrado com essa matrícula", 409);
        }

        var entity = MapCreate(dto);
        var criado = await _repository.Criar(entity, cancellationToken);
        return MapDto(criado);
    }

    public async Task<ImportResultDto> CriarEmLote(IEnumerable<(int linha, ColaboradorImportDto data)> colaboradores, CancellationToken cancellationToken = default)
    {
        var linhasComErro = new List<ImportErrorLineDto>();
        var criados = 0;
        var atualizados = 0;

        foreach (var (linha, data) in colaboradores)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(data.Matricula))
                {
                    throw new OrbiteOneException("Matrícula não informada");
                }

                var existente = await _repository.BuscarPorMatricula(data.Matricula, cancellationToken);
                if (existente != null)
                {
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

        var total = colaboradores.Count();
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

    public async Task<List<ColaboradorDto>> Listar(CancellationToken cancellationToken = default)
    {
        var itens = await _repository.Listar(cancellationToken);
        return itens.Select(MapDto).ToList();
    }

    public async Task<ColaboradorDto> BuscarPorMatricula(string matricula, CancellationToken cancellationToken = default)
    {
        var colaborador = await _repository.BuscarPorMatricula(matricula, cancellationToken);
        if (colaborador == null)
        {
            throw new OrbiteOneException("Colaborador não encontrado");
        }
        return MapDto(colaborador);
    }

    public async Task<ColaboradorDto> Atualizar(string matricula, ColaboradorUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var colaborador = await _repository.BuscarPorMatricula(matricula, cancellationToken);
        if (colaborador == null)
        {
            throw new OrbiteOneException("Colaborador não encontrado");
        }

        ApplyPatch(colaborador, dto);
        var atualizado = await _repository.Atualizar(colaborador, cancellationToken);
        return MapDto(atualizado);
    }

    public async Task Remover(string matricula, CancellationToken cancellationToken = default)
    {
        var colaborador = await _repository.BuscarPorMatricula(matricula, cancellationToken);
        if (colaborador == null)
        {
            throw new OrbiteOneException("Colaborador não encontrado");
        }
        await _repository.Remover(colaborador, cancellationToken);
    }

    private static Colaborador MapCreate(ColaboradorCreateDto dto)
    {
        return new Colaborador
        {
            Matricula = dto.Matricula,
            Nome = dto.Nome,
            Pis = dto.Pis,
            Cpf = dto.Cpf,
            DataAdmissao = dto.DataAdmissao,
            DataDemissao = dto.DataDemissao,
            NomeSetor = dto.NomeSetor,
            NomeCargo = dto.NomeCargo,
            NomeDepartamento = dto.NomeDepartamento,
            NomeUnidade = dto.NomeUnidade,
            NomeLotacao = dto.NomeLotacao,
            Situacao = dto.Situacao,
            CnpjUnidade = dto.CnpjUnidade,
            Ctps = dto.Ctps,
            Serie = dto.Serie,
        };
    }

    private static Colaborador MapImportToCreate(ColaboradorImportDto dto)
    {
        return new Colaborador
        {
            Matricula = dto.Matricula ?? string.Empty,
            Nome = dto.Nome ?? string.Empty,
            Pis = dto.Pis ?? string.Empty,
            Cpf = dto.Cpf ?? string.Empty,
            DataAdmissao = dto.DataAdmissao ?? default,
            DataDemissao = dto.DataDemissao,
            NomeSetor = dto.NomeSetor,
            NomeCargo = dto.NomeCargo,
            NomeDepartamento = dto.NomeDepartamento,
            NomeUnidade = dto.NomeUnidade ?? string.Empty,
            NomeLotacao = dto.NomeLotacao ?? string.Empty,
            Situacao = dto.Situacao ?? string.Empty,
            CnpjUnidade = dto.CnpjUnidade ?? string.Empty,
            Ctps = dto.Ctps,
            Serie = dto.Serie,
        };
    }

    private static void ApplyPatch(Colaborador colaborador, ColaboradorUpdateDto dto)
    {
        if (dto.Nome != null) colaborador.Nome = dto.Nome;
        if (dto.Pis != null) colaborador.Pis = dto.Pis;
        if (dto.Cpf != null) colaborador.Cpf = dto.Cpf;
        if (dto.DataAdmissao.HasValue) colaborador.DataAdmissao = dto.DataAdmissao.Value;
        if (dto.DataDemissao.HasValue) colaborador.DataDemissao = dto.DataDemissao.Value;
        if (dto.NomeSetor != null) colaborador.NomeSetor = dto.NomeSetor;
        if (dto.NomeCargo != null) colaborador.NomeCargo = dto.NomeCargo;
        if (dto.NomeDepartamento != null) colaborador.NomeDepartamento = dto.NomeDepartamento;
        if (dto.NomeUnidade != null) colaborador.NomeUnidade = dto.NomeUnidade;
        if (dto.NomeLotacao != null) colaborador.NomeLotacao = dto.NomeLotacao;
        if (dto.Situacao != null) colaborador.Situacao = dto.Situacao;
        if (dto.CnpjUnidade != null) colaborador.CnpjUnidade = dto.CnpjUnidade;
        if (dto.Ctps != null) colaborador.Ctps = dto.Ctps;
        if (dto.Serie != null) colaborador.Serie = dto.Serie;
    }

    private static void ApplyPatch(Colaborador colaborador, ColaboradorImportDto dto)
    {
        if (dto.Nome != null) colaborador.Nome = dto.Nome;
        if (dto.Pis != null) colaborador.Pis = dto.Pis;
        if (dto.Cpf != null) colaborador.Cpf = dto.Cpf;
        if (dto.DataAdmissao.HasValue) colaborador.DataAdmissao = dto.DataAdmissao.Value;
        if (dto.DataDemissao.HasValue) colaborador.DataDemissao = dto.DataDemissao.Value;
        if (dto.NomeSetor != null) colaborador.NomeSetor = dto.NomeSetor;
        if (dto.NomeCargo != null) colaborador.NomeCargo = dto.NomeCargo;
        if (dto.NomeDepartamento != null) colaborador.NomeDepartamento = dto.NomeDepartamento;
        if (dto.NomeUnidade != null) colaborador.NomeUnidade = dto.NomeUnidade;
        if (dto.NomeLotacao != null) colaborador.NomeLotacao = dto.NomeLotacao;
        if (dto.Situacao != null) colaborador.Situacao = dto.Situacao;
        if (dto.CnpjUnidade != null) colaborador.CnpjUnidade = dto.CnpjUnidade;
        if (dto.Ctps != null) colaborador.Ctps = dto.Ctps;
        if (dto.Serie != null) colaborador.Serie = dto.Serie;
    }

    private static ColaboradorDto MapDto(Colaborador colaborador)
    {
        return new ColaboradorDto
        {
            Matricula = colaborador.Matricula,
            Nome = colaborador.Nome,
            Pis = colaborador.Pis,
            Cpf = colaborador.Cpf,
            DataAdmissao = colaborador.DataAdmissao,
            DataDemissao = colaborador.DataDemissao,
            NomeSetor = colaborador.NomeSetor,
            NomeCargo = colaborador.NomeCargo,
            NomeDepartamento = colaborador.NomeDepartamento,
            NomeUnidade = colaborador.NomeUnidade,
            NomeLotacao = colaborador.NomeLotacao,
            Situacao = colaborador.Situacao,
            CnpjUnidade = colaborador.CnpjUnidade,
            Ctps = colaborador.Ctps,
            Serie = colaborador.Serie,
        };
    }
}
