using ModularApp.Modules.Workspace.Domain.Entities;

namespace ModularApp.Modules.Workspace.Application.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetUsersAsync(CancellationToken ct);
}