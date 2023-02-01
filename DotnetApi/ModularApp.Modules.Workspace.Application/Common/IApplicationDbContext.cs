using ModularApp.Modules.Workspace.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ModularApp.Modules.Workspace.Application.Common;

public interface IApplicationDbContext
{
    DbSet<Domain.Entities.Workspace> Workspaces { get; }
    DbSet<WorkspaceMember> WorkspaceMembers { get; }
    DbSet<User> Users { get; }
    DbSet<Project> Projects { get; }
    DbSet<ProjectMember> ProjectMembers { get; }
}