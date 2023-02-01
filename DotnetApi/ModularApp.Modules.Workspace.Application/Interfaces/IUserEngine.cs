using ModularApp.Modules.Workspace.Domain.Entities;

namespace ModularApp.Modules.Workspace.Application.Interfaces;

public interface IUserEngine
{
    Task<User> CreateUserAsync(CancellationToken ct);

    Task<List<User>> CreateUsersAsync(List<User> users, CancellationToken ct);
    
    Task<List<User>> TestCreateUsersAsync(CancellationToken ct);
}