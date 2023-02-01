using System.Security.Claims;
using ModularApp.Modules.Workspace.Application.Common;

namespace ModularApp.WebApi.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    // We 
    private Guid? _userId;
    public Guid? UserId =>  (Guid.TryParse(
        _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty,
        out var validGuid)
        ? validGuid
        : null);

    /*
     * Problem:
     * When running an Integration Test it 
     * We impersonate by using this method.
     */
    public void ImpersonateAsUser(Guid userId)
    {
        _userId = userId;
        
        var impersonatedUser = new ClaimsPrincipal(
            new ClaimsIdentity(
                new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                }));
        
        if (_httpContextAccessor.HttpContext is null)
        {
            _httpContextAccessor.HttpContext = new DefaultHttpContext
            {
                User = impersonatedUser

            };
            
            return;
        }

        _httpContextAccessor.HttpContext.User = impersonatedUser;
    }
}