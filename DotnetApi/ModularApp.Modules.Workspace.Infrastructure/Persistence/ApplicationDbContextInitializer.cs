using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ModularApp.Modules.Workspace.Domain.Entities;

namespace ModularApp.Modules.Workspace.Infrastructure.Persistence;

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
    
    /*
     * 'EnsureCreatedAsync' does **not** use migrations to create the database.
     * NB! In addition, the database that is created cannot be later updated using migrations.
     *
     * If you are targeting a relational database and using migrations 'MigrateAsync' to ensure the database is created using migrations and that all migrations have been applied.
     */
    public async Task InitialiseAsync(IConfiguration configuration)
    {
        try
        {
            if (!_context.Database.IsSqlServer())
                return;

            switch (configuration.GetValue<string>("DbInitializeMode"))
            {
                case "ResetOnEachStartupWithoutMigration":
                {
                    await _context.Database.EnsureDeletedAsync();
                    
                    var isCreated = await _context.Database.EnsureCreatedAsync();
                    if (isCreated) break;
                    
                    throw new Exception("Database not created.");
                }
                case "DoNotResetOnEachStartUpWithMigration":
                {
                    await _context.Database.MigrateAsync();
                    
                    break;
                }
                case "DoNotResetOnEachStartUpWithoutMigration":
                {
                    var isCreated = await _context.Database.EnsureCreatedAsync();
                    if (isCreated) break;
                    
                    throw new Exception("Database not created.");
                }
                default:
                {
                    var isCreated = await _context.Database.EnsureCreatedAsync();
                    if (isCreated) break;

                    throw new Exception("Database not created.");
                }
            }
            
            
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
            _context.Workspaces.Add(new Domain.Entities.Workspace
            {
                Name = "Default workspace"
            });

            await _context.SaveChangesAsync();
        }
    }
}