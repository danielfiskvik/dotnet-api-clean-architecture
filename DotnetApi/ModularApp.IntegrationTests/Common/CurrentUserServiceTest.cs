using ModularApp.Modules.Workspace.Application.Common;

namespace ModularApp.IntegrationTests.Common;

public class CurrentUserServiceTest : ICurrentUserService
{
    public Guid? UserId { get; }

    public CurrentUserServiceTest(Guid userId)
    {
        UserId = userId;
    }
    
    public void ImpersonateAsUser(Guid userId)
    {
        // None
    }
}