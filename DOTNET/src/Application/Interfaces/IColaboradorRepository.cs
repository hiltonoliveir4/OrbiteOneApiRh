using Domain.Entities;

namespace Application.Interfaces;

public interface IColaboradorRepository
{
    Task<Colaborador?> BuscarPorMatricula(string matricula, CancellationToken cancellationToken = default);
    Task<List<Colaborador>> Listar(CancellationToken cancellationToken = default);
    Task<Colaborador> Criar(Colaborador colaborador, CancellationToken cancellationToken = default);
    Task<Colaborador> Atualizar(Colaborador colaborador, CancellationToken cancellationToken = default);
    Task Remover(Colaborador colaborador, CancellationToken cancellationToken = default);
}
