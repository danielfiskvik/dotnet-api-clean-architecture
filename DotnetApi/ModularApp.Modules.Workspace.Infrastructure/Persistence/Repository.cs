using Microsoft.EntityFrameworkCore;
using ModularApp.Modules.Workspace.Application.Common;

namespace ModularApp.Modules.Workspace.Infrastructure.Persistence;

public class Repository : IRepository, IDisposable
{
    private readonly ApplicationDbContext _applicationDbContext;

    public Repository(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }
    
    public IQueryable<TEntity> Secure<TEntity>() where TEntity : class
        => _applicationDbContext.Set<TEntity>();
    
    public IQueryable<TEntity> SecureWithNoTracking<TEntity>() where TEntity : class
        => _applicationDbContext.Set<TEntity>().AsNoTracking();
    
    public IQueryable<TEntity> Unsecure<TEntity>() where TEntity : class
        => _applicationDbContext.Set<TEntity>().IgnoreQueryFilters();
    
    public IQueryable<TEntity> UnsecureWithNoTracking<TEntity>() where TEntity : class
        => _applicationDbContext.Set<TEntity>().AsNoTracking().IgnoreQueryFilters();
    
    public void Dispose()
    {
        _applicationDbContext.Dispose();
    }
}