namespace DotNetMovieApi.Repositories.Interfaces;

public interface IReadByIdRepository<TItem>
{
    Task<TItem?> SelectById(Guid id, CancellationToken cancellationToken = default);
}

public interface IFilterRepository<TCollection, in TFilter>
{
    Task<TCollection> Select(TFilter? filters = default, CancellationToken cancellationToken = default);
}

public interface ICreateRepository<in TRequest>
{
    Task<Guid> Create(TRequest request,  CancellationToken cancellationToken = default);
}

public interface IUpdateRepository<in TRequest>
{
    Task<bool> Update(Guid id, TRequest request, CancellationToken cancellationToken = default);
}

public interface IDeleteRepository
{
    Task<bool> Delete(Guid id, CancellationToken cancellationToken = default);
}
