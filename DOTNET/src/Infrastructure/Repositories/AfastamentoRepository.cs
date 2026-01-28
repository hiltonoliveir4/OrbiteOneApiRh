using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AfastamentoRepository : IAfastamentoRepository
{
    private readonly AppDbContext _db;

    public AfastamentoRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<Afastamento?> BuscarPorId(int id, CancellationToken cancellationToken = default)
    {
        return _db.Afastamentos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<List<Afastamento>> Listar(CancellationToken cancellationToken = default)
    {
        return _db.Afastamentos.AsNoTracking().ToListAsync(cancellationToken);
    }

    public Task<List<Afastamento>> ListarPorMatricula(string matricula, CancellationToken cancellationToken = default)
    {
        return _db.Afastamentos.AsNoTracking().Where(x => x.Matricula == matricula).ToListAsync(cancellationToken);
    }

    public async Task<Afastamento> Criar(Afastamento afastamento, CancellationToken cancellationToken = default)
    {
        _db.Afastamentos.Add(afastamento);
        await _db.SaveChangesAsync(cancellationToken);
        return afastamento;
    }

    public async Task<Afastamento> Atualizar(Afastamento afastamento, CancellationToken cancellationToken = default)
    {
        _db.Afastamentos.Update(afastamento);
        await _db.SaveChangesAsync(cancellationToken);
        return afastamento;
    }

    public async Task Remover(Afastamento afastamento, CancellationToken cancellationToken = default)
    {
        _db.Afastamentos.Remove(afastamento);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
