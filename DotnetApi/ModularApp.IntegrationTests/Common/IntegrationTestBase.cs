using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using ModularApp.Modules.Workspace.Application.Common;
using ModularApp.Modules.Workspace.Infrastructure.Persistence;

namespace ModularApp.IntegrationTests.Common;

public class IntegrationTestBase : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    
    private Guid? _currentUserId;
    private int? _existingUsersCount;

    protected IntegrationTestBase(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    protected async Task<HttpClient> InitializeTestAndCreateHttpClientAsync()
    {
        var currentUserId = await CurrentUserIdAsync();

        var factory =  _factory
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddScoped<ICurrentUserService>(_ => new CurrentUserServiceTest(currentUserId));
                });

                builder.UseSetting("DbInitializeMode", "ResetOnEachStartupWithoutMigration");
            })
            .CreateClient();

        return factory;
    }

    protected int ExistingUserCountSnapshot => _existingUsersCount ?? 0;

    protected Guid CurrentUserIdSnapshot => _currentUserId ?? Guid.Empty;
    
    private async Task<Guid> CurrentUserIdAsync()
    {
        if (_currentUserId is not null)
            return _currentUserId.Value;

        using var scope = _factory.Services.CreateScope();

        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();
        await initializer.InitialiseAsync(configuration);
        await initializer.SeedAsync();
        
        var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();

        if (unitOfWork == null)
            return Guid.Empty;
        
        _currentUserId = await unitOfWork.ApplicationDbContext.Users
            .Where(x => x.UserName == "UserName")
            .Select(x => x.Id)
            .FirstOrDefaultAsync();

        _existingUsersCount = await unitOfWork.ApplicationDbContext.Users.CountAsync();
            
        return _currentUserId.Value;
    }
}