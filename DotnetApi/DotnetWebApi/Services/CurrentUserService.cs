using System.Security.Claims;
using Application.Common;

namespace DotnetWebApi.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId => Guid.TryParse(
        _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty,
        out var validGuid)
        ? validGuid
        : null;

    public void ImpersonateAsUser(Guid userId)
    {
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