using ModularApp.Modules.Workspace.Application.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ModularApp.Core.Extensions;
using ModularApp.Modules.Workspace.Domain.Interfaces;
using ModularApp.Modules.Workspace.Infrastructure.Persistence.Extensions;

namespace ModularApp.Modules.Workspace.Infrastructure.Persistence;

/*
 * Purpose of the "Unit of Work Design Pattern":
 * - Keep track of multiple database operations, such as insert, update, and delete,
 *   and ensure that all of these operations are executed in a single, atomic transaction.
 *   This means that if any of the operations fail, all of the previous operations will be rolled back,
 *   so the database remains in a consistent state.
 * - Can be used to provide a consistent and easy-to-use interface for working with the database,
 *   and to abstract away the details of the underlying database technology.
 *
 * As you can see the UnitOfWork pattern is used in a database context.
 * It is a good pattern to use if you want to add business logic when a entry state is changed.
 *
 * The Repository and Service classes can interact with the UnitOfWork class to perform database operations.
 *
 * Does Entity Framework (EF) itself have an explicit implementation of the Unit of Work pattern built-in?
 * - Short answer is no.
 *
 * The DbContext class in EF is often used as a Unit of Work, it keeps track of all the changes made to the entities during the lifetime of the context,
 * and when the SaveChanges method is called, it persists all the changes to the database in a single transaction.
 * 
 */
internal sealed class UnitOfWork : IUnitOfWork, IDisposable
{
    /*
     * UnitOfWork wraps "DbContext" object, which is the primary class for interacting with database.
     *
     * DbContext is not thread-safe and therefore it is not designed to be used by multiple threads simultaneously.
     * Each instance of DbContext maintains its own state and caching of entities,
     * so if multiple threads try to access the same context instance, it can lead to unexpected behavior and data inconsistencies.
     *
     * Additionally, DbContext uses a underlying connection to the database that is also not thread safe.
     * If multiple threads try to access the same connection, it can lead to connection pool exhaustion, Deadlock and other issues.
     *
     * In a multi-threaded application, it is recommended to use a separate instance of DbContext per thread,
     * or to use a thread-safe synchronization mechanism such as a lock when accessing the DbContext.
     *
     * It's also important to note that the DbContext should be disposed of properly at the end of the request,
     * otherwise it can lead to memory leaks and other issues.
     */
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly ICurrentUserService _currentUserService;

    private IDbContextTransaction? _dbContextTransaction;

    public UnitOfWork(ApplicationDbContext applicationDbContext, ICurrentUserService currentUserService)
    {
        _applicationDbContext = applicationDbContext;
        _currentUserService = currentUserService;
    }

    public IApplicationDbContext ApplicationDbContext => _applicationDbContext;

    /*
     * SaveChanges method, which is used to persist any changes made to the database.
     * The SaveChanges method is responsible for persisting any changes made to the entities in the database.
     * 
     * The SaveChanges method can be called multiple times during the lifetime of the UnitOfWork,
     * but it will only persist all the changes to the database once, at the end of the UnitOfWork lifetime.
     *
     * It's important to note that the SaveChanges method should be called only once during the lifetime of the UnitOfWork object,
     * as it ensures that all the operations are executed in a single, atomic transaction.
     */
    public async Task<int> SaveChangesAsync(CancellationToken ct)
    {
        /*
         * TODO Put logic here from "SaveChangesInterceptors" such as "EntityFrameworkSaveChangesInterceptor".
         */
        try
        {
            return await _applicationDbContext.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            // Handle concurrency exception
        }
        catch (DbUpdateException)
        {
            // Handle update exception
        }
        catch (Exception)
        {
            // Handle validation exception
        }

        return default;
    }

    #region UnitOfWork
    public async Task BeginTransactionAsync(CancellationToken ct)
    {
        // Transactions are not supported by the in-memory store.
        if (_applicationDbContext.Database.IsInMemory())
            return;
        
        _dbContextTransaction = await _applicationDbContext.Database.BeginTransactionAsync(ct);
    }

    public async Task CommitTransactionAsync(CancellationToken ct)
    {
        // Transactions are not supported by the in-memory store.
        if (_applicationDbContext.Database.IsInMemory())
            return;
        
        if (_dbContextTransaction is null)
            return;
        
        await _dbContextTransaction.CommitAsync(ct);
    }

    public async Task RollbackTransactionAsync(CancellationToken ct)
    {
        // Transactions are not supported by the in-memory store.
        if (_applicationDbContext.Database.IsInMemory())
            return;
        
        if (_dbContextTransaction is null)
            return;
        
        await _dbContextTransaction.RollbackAsync(ct);
    }
    #endregion

    #region Create, Update, Delete
    public async Task<TEntity> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken)
        where TEntity : class
    {
        await _applicationDbContext
            .Set<TEntity>()
            .AddAsync(entity, cancellationToken);

        return entity;
    }
    
    public async Task<List<TEntity>> AddRangeAsync<TEntity>(List<TEntity> entities, CancellationToken cancellationToken)
        where TEntity : class
    {
        if (!entities.Any())
            return entities;
        
        entities.Reverse();

        await _applicationDbContext
            .Set<TEntity>()
            .AddRangeAsync(entities, cancellationToken);

        return entities;
    }
    
    public TEntity Update<TEntity>(TEntity entity)
        where TEntity : class
    {
        _applicationDbContext
            .Set<TEntity>()
            .Entry(entity).State = EntityState.Modified;

        return entity;
    }

    public async Task<TEntity> UpdateAndSaveAsync<TEntity>(TEntity entity, CancellationToken cancellationToken)
        where TEntity : class
    {
        _applicationDbContext
            .Set<TEntity>()
            .Entry(entity).State = EntityState.Modified;

        await SaveChangesAsync(cancellationToken);

        return entity;
    }
    
    public void RemoveAsync<TEntity>(TEntity? entity, bool hardDelete)
        where TEntity : class
    {
        if (entity is null)
            return;

        if (!hardDelete)
        {
            if (!entity.IsImplementingFrom(x => x == typeof(ISoftDeletable)))
                return;

            var userId = _currentUserService.UserId;

            entity.SetSoftDeletablePropertiesOnEntity(userId);

            return;
        }

        _applicationDbContext
            .Set<TEntity>()
            .Remove(entity);
    }
    #endregion

    public void Dispose()
    {
        _dbContextTransaction?.Dispose();

        _applicationDbContext.Dispose();
    }
}