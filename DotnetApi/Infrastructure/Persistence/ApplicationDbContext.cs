using System.Reflection;
using Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public partial class ApplicationDbContext : DbContext
{
    private readonly EntityFrameworkSaveChangesInterceptor _entityFrameworkSaveChangesInterceptor;
    
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        EntityFrameworkSaveChangesInterceptor entityFrameworkSaveChangesInterceptor) 
        : base(options)
    {
        _entityFrameworkSaveChangesInterceptor = entityFrameworkSaveChangesInterceptor;
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_entityFrameworkSaveChangesInterceptor);
    }
}