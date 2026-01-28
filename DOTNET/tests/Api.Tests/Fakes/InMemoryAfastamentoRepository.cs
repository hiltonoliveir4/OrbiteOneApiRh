using Application.Interfaces;
using Domain.Entities;

namespace Api.Tests.Fakes;

public class InMemoryAfastamentoRepository : IAfastamentoRepository
{
    private readonly List<Afastamento> _items = new();
    private int _nextId = 1;

    public Task<Afastamento?> BuscarPorId(int id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_items.FirstOrDefault(x => x.Id == id));
    }

    public Task<List<Afastamento>> Listar(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_items.ToList());
    }

    public Task<List<Afastamento>> ListarPorMatricula(string matricula, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_items.Where(x => x.Matricula == matricula).ToList());
    }

    public Task<Afastamento> Criar(Afastamento afastamento, CancellationToken cancellationToken = default)
    {
        afastamento.Id = _nextId++;
        _items.Add(afastamento);
        return Task.FromResult(afastamento);
    }

    public Task<Afastamento> Atualizar(Afastamento afastamento, CancellationToken cancellationToken = default)
    {
        var existing = _items.FirstOrDefault(x => x.Id == afastamento.Id);
        if (existing != null)
        {
            _items.Remove(existing);
            _items.Add(afastamento);
        }
        return Task.FromResult(afastamento);
    }

    public Task Remover(Afastamento afastamento, CancellationToken cancellationToken = default)
    {
        _items.RemoveAll(x => x.Id == afastamento.Id);
        return Task.CompletedTask;
    }
}
