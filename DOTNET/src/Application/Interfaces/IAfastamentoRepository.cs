using Domain.Entities;

namespace Application.Interfaces;

public interface IAfastamentoRepository
{
    Task<Afastamento?> BuscarPorId(int id, CancellationToken cancellationToken = default);
    Task<List<Afastamento>> Listar(CancellationToken cancellationToken = default);
    Task<List<Afastamento>> ListarPorMatricula(string matricula, CancellationToken cancellationToken = default);
    Task<Afastamento> Criar(Afastamento afastamento, CancellationToken cancellationToken = default);
    Task<Afastamento> Atualizar(Afastamento afastamento, CancellationToken cancellationToken = default);
    Task Remover(Afastamento afastamento, CancellationToken cancellationToken = default);
}
