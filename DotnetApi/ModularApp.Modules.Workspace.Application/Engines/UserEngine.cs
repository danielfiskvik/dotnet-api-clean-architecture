using ModularApp.Modules.Workspace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Faker;
using ModularApp.Modules.Workspace.Application.Common;
using ModularApp.Modules.Workspace.Application.Interfaces;

namespace ModularApp.Modules.Workspace.Application.Engines;

/*
     * TODO - Discovery: Can the the transaction be handled in a "Railway-pattern"? Example:
     *
     * railwaySteps
     * .BeginTransaction()
     * .PerformBusinessLogic(x => {... business logic here })
     * .SaveAndCommitTransaction()
     * .IfException(x => RollbackTransactionHandler)
     * .ThenReturn(FaultyObject)
     * .OrElse(x => x.applicationDbContext.Users.AsNoTracking())
     */

// #region Testing if IsModified is working
// var numberOfUsers = await _unitOfWork.ApplicationDbContext.Users.CountAsync(ct);
// // ReSharper disable once InvertIf
// if (numberOfUsers > 5)
// {
//     user.FullName = $"User {numberOfUsers} is modified!";
//     
//     await _unitOfWork.SaveChangesAsync(ct);
// }
// #endregion
public class UserEngine : IUserEngine
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public UserEngine(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<User> CreateUserAsync(CancellationToken ct)
    {
        var user = new User
        {
            UserName = Name.FullName(),
            FirstName = Name.First(),
            SureName = Name.Last(),
        };

        await _unitOfWork.AddAsync(user, ct);
        
        await _unitOfWork.SaveChangesAsync(ct);

        return user;
    }

    
    public async Task<List<User>> CreateUsersAsync(List<User> users, CancellationToken ct)
    {
        // Set Fullname
        foreach (var user in users.Where(user => string.IsNullOrWhiteSpace(user.FullName)))
        {
            user.FullName = $"{user.FirstName} {user.SureName}";
        }
        
        try
        {
            await _unitOfWork.BeginTransactionAsync(ct);
            try
            {
                await _unitOfWork.AddRangeAsync(users, ct);

                await _unitOfWork.SaveChangesAsync(ct);

                await _unitOfWork.CommitTransactionAsync(ct);
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync(ct);
            
                // TODO What to do when exception? Make a standard way to return a Result object with fault state.

                throw;
            }
        }
        catch (Exception e)
        {
            var message = e.ToString();
            // TODO what to do if cancellation token on BeginTransactionAsync?
            
            throw new Exception(message);
        }
        
        return users;
    }
    
    /*
     * Simulating that Transaction pattern works with UnitOfWork
     */
    public async Task<List<User>> TestCreateUsersAsync(CancellationToken ct)
    {
        var impersonateUserId = await _unitOfWork.ApplicationDbContext.Users
            .AsNoTracking()
            .Select(x => x.Id)
            .FirstOrDefaultAsync(ct);
        
        _currentUserService.ImpersonateAsUser(impersonateUserId);

        var users = new List<User>();
        
        await _unitOfWork.BeginTransactionAsync(default);
        try
        {
            for (var i = 0; i < 10; i++)
            {
                var user = new User
                {
                    UserName = Name.FullName(),
                    FirstName = Name.First(),
                    SureName = Name.Last(),
                };
                

                await _unitOfWork.ApplicationDbContext.Users
                    .AddAsync(user, ct);
                
                users.Add(user);

                await Task.Delay(2000, ct);
            }

            await _unitOfWork.SaveChangesAsync(ct);

            await _unitOfWork.CommitTransactionAsync(ct);
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackTransactionAsync(ct);
        }

        return users;
    }
}