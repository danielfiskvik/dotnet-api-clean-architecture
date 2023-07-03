using ModularApp.Modules.Workspace.Infrastructure.Persistence;

namespace ModularApp.WebApi.Configurations;

public static class PersistenceSetup
{
    public static async Task InitializePersistenceModuleAsync(this WebApplication app, IConfiguration configuration)
    {
        using var scope = app.Services.CreateScope();
        var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();
        
        // Initialise and seed database
        await initializer.InitialiseAsync(configuration);
        await initializer.SeedAsync();
    }
}