using LicenseService.API.Application.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
namespace LicenseService.API.Infrastructure.Identity
{  
    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? UserId =>
            _httpContextAccessor.HttpContext?.User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name)
            ?? "unknown";

        public Guid TenantId
        {
            get
            {
                var tenantClaim = _httpContextAccessor.HttpContext?.User.FindFirst("tenant_id")?.Value;
                return Guid.TryParse(tenantClaim, out var tid) ? tid : Guid.Empty;
            }
        }
    }

}
