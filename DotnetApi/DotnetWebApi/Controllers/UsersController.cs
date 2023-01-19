using Application.Common;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotnetWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly ICurrentUserService _currentUserService;

    public UsersController(
        ApplicationDbContext applicationDbContext,
        ICurrentUserService currentUserService)
    {
        _applicationDbContext = applicationDbContext;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    public async Task<IEnumerable<User>> GetUsersAsync(CancellationToken ct)
    {
        return await _applicationDbContext.Users
            .AsNoTracking()
            .ToListAsync(ct);
    }
    
    [HttpPost]
    public async Task<IQueryable<User>> CreateUserAsync(CancellationToken ct)
    {
        var impersonateUserId = await _applicationDbContext.Users
            .AsNoTracking()
            .Select(x => x.Id)
            .FirstOrDefaultAsync(ct);
        
        _currentUserService.ImpersonateAsUser(impersonateUserId);
        
        var user = new User
        {
            UserName = Faker.Name.FullName(),
            FirstName = Faker.Name.First(),
            SureName = Faker.Name.Last(),
        };

        await _applicationDbContext.Users.AddAsync(user, ct);
        await _applicationDbContext.SaveChangesAsync(ct);

        var numberOfUsers = await _applicationDbContext.Users.CountAsync(ct);
        // ReSharper disable once InvertIf
        if (numberOfUsers > 5)
        {
            user.FullName = $"User {numberOfUsers} is modified!";
            
            await _applicationDbContext.SaveChangesAsync(ct);
        }
        
        return _applicationDbContext.Users.AsNoTracking();
    }
}