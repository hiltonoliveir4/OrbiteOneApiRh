using Application.Interfaces;
using Domain.Entities;

namespace Api.Tests.Fakes;

public class InMemoryColaboradorRepository : IColaboradorRepository
{
    private readonly List<Colaborador> _items = new();
    private int _nextId = 1;

    public Task<Colaborador?> BuscarPorMatricula(string matricula, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_items.FirstOrDefault(x => x.Matricula == matricula));
    }

    public Task<List<Colaborador>> Listar(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_items.ToList());
    }

    public Task<Colaborador> Criar(Colaborador colaborador, CancellationToken cancellationToken = default)
    {
        colaborador.Id = _nextId++;
        _items.Add(colaborador);
        return Task.FromResult(colaborador);
    }

    public Task<Colaborador> Atualizar(Colaborador colaborador, CancellationToken cancellationToken = default)
    {
        var existing = _items.FirstOrDefault(x => x.Id == colaborador.Id);
        if (existing != null)
        {
            _items.Remove(existing);
            _items.Add(colaborador);
        }
        return Task.FromResult(colaborador);
    }

    public Task Remover(Colaborador colaborador, CancellationToken cancellationToken = default)
    {
        _items.RemoveAll(x => x.Id == colaborador.Id);
        return Task.CompletedTask;
    }
}
