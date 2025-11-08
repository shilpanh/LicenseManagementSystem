using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
namespace LicenseService.API.Infrastructure.Tenant
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;
        public TenantMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context, ITenantProvider tenantProvider)
        {
            // resolve tenant from header or JWT claim already handled in provider
            var tenantId = tenantProvider.GetTenantId();
            if (string.IsNullOrEmpty(tenantId) && context.Request.Headers.ContainsKey("X-Tenant-ID"))
            {
                tenantProvider.SetTenantId(context.Request.Headers["X-Tenant-ID"].First());
            }

            await _next(context);
        }
    }

}
