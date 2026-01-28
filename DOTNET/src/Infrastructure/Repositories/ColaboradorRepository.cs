using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ColaboradorRepository : IColaboradorRepository
{
    private readonly AppDbContext _db;

    public ColaboradorRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<Colaborador?> BuscarPorMatricula(string matricula, CancellationToken cancellationToken = default)
    {
        return _db.Colaboradores.AsNoTracking().FirstOrDefaultAsync(x => x.Matricula == matricula, cancellationToken);
    }

    public Task<List<Colaborador>> Listar(CancellationToken cancellationToken = default)
    {
        return _db.Colaboradores.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<Colaborador> Criar(Colaborador colaborador, CancellationToken cancellationToken = default)
    {
        _db.Colaboradores.Add(colaborador);
        await _db.SaveChangesAsync(cancellationToken);
        return colaborador;
    }

    public async Task<Colaborador> Atualizar(Colaborador colaborador, CancellationToken cancellationToken = default)
    {
        _db.Colaboradores.Update(colaborador);
        await _db.SaveChangesAsync(cancellationToken);
        return colaborador;
    }

    public async Task Remover(Colaborador colaborador, CancellationToken cancellationToken = default)
    {
        _db.Colaboradores.Remove(colaborador);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
