using System.Reflection;
using ModularApp.Modules.Workspace.Application.Common;
using Microsoft.EntityFrameworkCore;
using ModularApp.Modules.Workspace.Infrastructure.Persistence.Interceptors;

namespace ModularApp.Modules.Workspace.Infrastructure.Persistence;

public partial class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly UpdateEntitiesByEntityStatusSaveChangesInterceptor _updateEntitiesByEntityStatusSaveChangesInterceptor;
    
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        UpdateEntitiesByEntityStatusSaveChangesInterceptor updateEntitiesByEntityStatusSaveChangesInterceptor) 
        : base(options)
    {
        _updateEntitiesByEntityStatusSaveChangesInterceptor = updateEntitiesByEntityStatusSaveChangesInterceptor;
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_updateEntitiesByEntityStatusSaveChangesInterceptor);
    }
}