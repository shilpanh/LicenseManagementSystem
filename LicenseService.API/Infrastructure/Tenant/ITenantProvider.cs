namespace LicenseService.API.Infrastructure.Tenant
{
    public interface ITenantProvider
    {
        string GetTenantId();
        void SetTenantId(string tenantId);
    }

}
