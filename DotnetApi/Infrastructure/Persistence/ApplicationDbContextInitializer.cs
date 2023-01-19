using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence;

public class ApplicationDbContextInitializer
{
    private readonly ILogger<ApplicationDbContextInitializer> _logger;
    private readonly ApplicationDbContext _context;

    public ApplicationDbContextInitializer(
        ILogger<ApplicationDbContextInitializer> logger,
        ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    
    public async Task InitialiseAsync(IConfiguration configuration)
    {
        try
        {
            if (!_context.Database.IsSqlServer())
                return;
            
            if (configuration.GetValue<bool>("DatabaseEnsureDeleted"))
                await _context.Database.EnsureDeletedAsync();

            await _context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database");
            
            throw;
        }
    }
    
    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database");
            
            throw;
        }
    }
    
    private async Task TrySeedAsync()
    {
        // TODO - Default roles
        
        // Default users
        var user = new User
        {
            UserName = "UserName",
            FirstName = "FirstName",
            SureName = "SureName",
            FullName = "FirstName SureName"
        };

        if (!_context.Users.Any(u => u.UserName != user.UserName))
        {
            await _context.Users.AddAsync(user);

            await _context.SaveChangesAsync();
        }

        // Default data
        // Seed, if necessary
        if (!_context.Workspaces.Any())
        {
            _context.Workspaces.Add(new Workspace
            {
                Name = "Default workspace"
            });

            await _context.SaveChangesAsync();
        }
    }
}