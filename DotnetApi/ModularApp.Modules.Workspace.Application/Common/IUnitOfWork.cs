namespace ModularApp.Modules.Workspace.Application.Common;

public interface IUnitOfWork
{
    IApplicationDbContext ApplicationDbContext { get; }

    Task<int> SaveChangesAsync(CancellationToken ct);
    
    #region UnitOfWork
    Task BeginTransactionAsync(CancellationToken ct);

    Task CommitTransactionAsync(CancellationToken ct);

    Task RollbackTransactionAsync(CancellationToken ct);
    #endregion

    #region Create, Update, Delete
    Task<TEntity> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken)
        where TEntity : class;

    Task<List<TEntity>> AddRangeAsync<TEntity>(List<TEntity> entities, CancellationToken cancellationToken)
        where TEntity : class;

    TEntity Update<TEntity>(TEntity entity)
        where TEntity : class;

    Task<TEntity> UpdateAndSaveAsync<TEntity>(TEntity entity, CancellationToken cancellationToken)
        where TEntity : class;

    void RemoveAsync<TEntity>(TEntity? entity, bool hardDelete)
        where TEntity : class;
    
    #endregion
}