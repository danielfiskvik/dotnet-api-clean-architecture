using Microsoft.Extensions.DependencyInjection;
using ModularApp.Modules.Workspace.Application.Engines;
using ModularApp.Modules.Workspace.Application.Interfaces;
using ModularApp.Modules.Workspace.Application.Repositories;

namespace ModularApp.Modules.Workspace.Application;

public static class ConfigureModule
{
    public static IServiceCollection AddApplicationModule(this IServiceCollection services)
    {
        services.AddScoped<ICharacterEngine, CharacterEngine>();
        services.AddScoped<ICharacterRepository, CharacterRepository>();
        
        return services;
    }
}