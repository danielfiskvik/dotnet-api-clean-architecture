using ModularApp.Modules.Workspace.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using ModularApp.Modules.Workspace.Application.Common;
using ModularApp.Modules.Workspace.Application.Interfaces;

namespace ModularApp.Modules.Workspace.Application.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IRepository _repository;

    public UserRepository(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<User>> GetUsersAsync(CancellationToken ct)
    {
        return await _repository
            .SecureWithNoTracking<User>()
            .ToListAsync(ct);
    }
}