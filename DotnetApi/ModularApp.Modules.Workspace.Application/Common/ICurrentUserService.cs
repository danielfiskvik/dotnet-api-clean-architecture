namespace ModularApp.Modules.Workspace.Application.Common;

public interface ICurrentUserService
{
    Guid? UserId { get; }

    void ImpersonateAsUser(Guid userId);
}