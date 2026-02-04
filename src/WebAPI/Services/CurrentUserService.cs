using System.Security.Claims;
using Inventory.Application.Interfaces;

namespace Inventory.WebAPI.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            var sub = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                   ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;
            return Guid.TryParse(sub, out var userId) ? userId : Guid.Empty;
        }
    }
}
