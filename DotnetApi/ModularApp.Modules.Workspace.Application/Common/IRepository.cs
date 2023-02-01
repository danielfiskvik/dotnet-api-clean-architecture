namespace ModularApp.Modules.Workspace.Application.Common;

public interface IRepository
{
    IQueryable<TEntity> Secure<TEntity>() where TEntity : class;

    IQueryable<TEntity> SecureWithNoTracking<TEntity>() where TEntity : class;

    IQueryable<TEntity> Unsecure<TEntity>() where TEntity : class;

    IQueryable<TEntity> UnsecureWithNoTracking<TEntity>() where TEntity : class;
}