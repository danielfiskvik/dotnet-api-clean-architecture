using ModularApp.Modules.Workspace.Application.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using ModularApp.Modules.Workspace.Application.Interfaces;
using ModularApp.Modules.Workspace.Infrastructure.Integrations;
using ModularApp.Modules.Workspace.Infrastructure.Persistence;
using ModularApp.Modules.Workspace.Infrastructure.Persistence.Interceptors;

namespace ModularApp.Modules.Workspace.Infrastructure;

/*
 * Facts about DI lifetimes such as Singleton, Scoped, and Transient, in the context of a Request lifetime. 
 *
 * Scoped:
 * Scoped lifetime creates a new instance of the object for each request,
 * so that each request has its own instance of the object that is unique to that request and it is disposed of at the end of the request.
 * 
 * Transient:
 * Transient lifetime creates a new instance of the object each time it is requested by the dependency injection container,
 * so that each time a new instance of the object is returned.
 * The instances are not shared between requests and they are not guaranteed to be the same instance across multiple requests.
 *
 *
 * 
 * Object lifetime in context of the Web Request lifetime:
 *
 * Scoped:
 * Regarding the lifetime of Scoped, it's created once per web request, or per thread in the case of a background worker,
 * and it's alive during the entire request, so that it's possible to access the same instance from different services,
 * repositories and controllers in the same request.
 *
 * Transient:
 * Regarding the lifetime of Transient, it's created every time it's requested, so that it's a new instance each time,
 * it's not shared between requests, and it's not guaranteed to be the same instance across multiple requests,
 * this could be useful when you want to avoid state sharing between requests.
 */
public static class ConfigureModule
{
    public static IServiceCollection AddPersistenceModule(
        this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration.GetValue<bool>("UseInMemoryDatabase"))
        {
            services.AddDbContext<ApplicationDbContext>(options => options
                .UseInMemoryDatabase("CleanArchitectureDb")
            );
        }
        else
        {
            services.AddDbContext<ApplicationDbContext>(options => options
                    .UseSqlServer(
                        configuration.GetConnectionString("TestConnection"),
                        o =>  o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
                        //, builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
                    )
                // .AddInterceptors()
            );
        }

        services.ConfigureHttpClients();
        
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        
        services.AddScoped<UpdateEntitiesByEntityStatusSaveChangesInterceptor>();

        services.AddScoped<ApplicationDbContextInitializer>();

        /*
         * Problem:
         * GraphQL can execute many concurrent operations toward DbContext within the same Request.
         * Since DbContext is not thread safe it should not be shared across multiple threads.
         * This means that having a Scoped lifetime for the DbContext / UnitDbWork object could potentially cause issues
         * if the same context instance is accessed by multiple threads at the same time.
         *
         * Solution:
         * A Transient lifetime for the UnitOfWork object creates a new context instance for each request,
         * which ensures that each request has its own context instance and that the context is not shared across multiple threads.
         * This can help to avoid threading issues and ensure that the application is stable.
         *
         * Drawbacks:
         * It's important to note that the Transient lifetime of the UnitOfWork object can lead to performance issues,
         * especially with high-load applications, as creating and disposing context object on each request can be expensive.
         *
         * If you are sure that the UnitOfWork will not be used by multiple threads and you are ok with the performance overhead then you can use a Scoped lifetime.
         * Otherwise, using a Transient lifetime would be a safer option in order to avoid threading issues.
         *
         * Important:
         * It's important to ensure that the context is disposed of properly at the end of the request, otherwise it can lead to memory leaks and other issues
         *
         * TODO:
         * Maybe add a flag in configuration that if graphql is used then DI should be Transient, if not UnitOfWork should be Scoped.
         */
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IRepository, Repository>();
        
        services.AddScoped<IWaniKaniIntegrationService, WaniKaniIntegrationService>();
        
        return services;
    }

    private static IServiceCollection ConfigureHttpClients(this IServiceCollection services)
    {
        services
            .AddHttpClient("WaniKaniHttpClient", httpClient =>
            {
                httpClient.BaseAddress = new Uri("https://www.wanikani.com");

                // using Microsoft.Net.Http.Headers;
                // The GitHub API requires two headers.
                // httpClient.DefaultRequestHeaders.Add(
                //     HeaderNames.Accept, "application/vnd.github.v3+json");
                // httpClient.DefaultRequestHeaders.Add(
                //     HeaderNames.UserAgent, "HttpRequestsSample");
            });

        return services;
    }
}