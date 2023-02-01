using ModularApp.Modules.Workspace.Domain.Entities;

namespace ModularApp.Modules.Workspace.Application.Extensions;

public static class UserExtension
{
    public static IQueryable<User> GetCurrentUserQueryable(this IQueryable<User> users, Guid userId)
        => users.Where(x => x.Id == userId);
}