using Microsoft.AspNetCore.Http;

namespace LicenseService.API.Infrastructure.Tenant
{
    public class HeaderTenantProvider : ITenantProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string TenantHeader = "X-Tenant-ID";
        private string _tenantId = null;

        public HeaderTenantProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetTenantId()
        {
            if (!string.IsNullOrEmpty(_tenantId)) return _tenantId;

            var ctx = _httpContextAccessor.HttpContext;
            // Match JWT claim name "tenant_id"
            if (ctx?.User?.HasClaim(c => c.Type == "tenant_id") == true)
            {
                _tenantId = ctx.User.FindFirst("tenant_id").Value;
                return _tenantId;
            }

            // Fallback to header if claim is missing
            const string TenantHeader = "X-Tenant-ID";
            if (ctx?.Request?.Headers?.ContainsKey(TenantHeader) == true)
            {
                _tenantId = ctx.Request.Headers[TenantHeader].First();
            }
                        
            return _tenantId;
        }

        public void SetTenantId(string tenantId) => _tenantId = tenantId;
    }

}
