using ModularApp.Modules.Workspace.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModularApp.Modules.Workspace.Application.Common;
using ModularApp.Modules.Workspace.Application.Extensions;
using ModularApp.Modules.Workspace.Domain.Entities;

namespace ModularApp.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IUserEngine _userEngine;
    private readonly IRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public UsersController(
        IUserRepository userRepository,
        IUserEngine userEngine,
        IRepository repository,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _userEngine = userEngine;
        _repository = repository;
        _currentUserService = currentUserService;
    }
    
    [HttpGet("Current")]
    public async Task<User?> GetCurrentUserAsync(CancellationToken ct)
    {
        var userId = _currentUserService.UserId ?? Guid.Empty;

        return await _repository
            .SecureWithNoTracking<User>()
            .GetCurrentUserQueryable(userId)
            .FirstOrDefaultAsync(ct);
    }

    [HttpGet]
    public async Task<IEnumerable<User>> GetUsersAsync(CancellationToken ct)
    {
        return await _userRepository.GetUsersAsync(ct);
    }
    
    [HttpPost("TestUnitOfWorkWithTransactionPattern")]
    public async Task<IQueryable<User>> CreateUsersTestAsync(CancellationToken ct)
    {
        await _userEngine.TestCreateUsersAsync(ct);

        return _repository.SecureWithNoTracking<User>();
    }
    
    [HttpPost]
    public async Task<IQueryable<User>> CreateUsersAsync([FromBody] List<User> users, CancellationToken ct)
    {
        await _userEngine.CreateUsersAsync(users, ct);
        
        return _repository.SecureWithNoTracking<User>();
    }
    
    [HttpPost("TestSaveChangesInterceptor")]
    public async Task<IQueryable<User>> TestCreateUsersAsync([FromBody] List<User> users, CancellationToken ct)
    {
        await _userEngine.CreateUsersAsync(users, ct);
        
        return _repository.SecureWithNoTracking<User>();
    }
}