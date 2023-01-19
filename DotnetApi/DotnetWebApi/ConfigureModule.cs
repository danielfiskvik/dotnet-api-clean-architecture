using Application.Common;
using DotnetWebApi.Services;

namespace DotnetWebApi;

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