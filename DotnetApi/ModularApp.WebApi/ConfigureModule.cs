using ModularApp.Modules.Workspace.Application.Common;
using ModularApp.WebApi.Services;

namespace ModularApp.WebApi;

public static class ConfigureModule
{
    public static IServiceCollection AddWebApiModule(
        this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddSingleton<ICurrentUserService, CurrentUserService>();
        
        return services;
    }
}