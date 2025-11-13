using global::LicenseService.API.Data;
using global::LicenseService.API.Domain;
using global::LicenseService.API.Infrastructure.Tenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
namespace LicenseService.API.Application.Services
{ 
        public class LicenseRenewalService
        {
            private readonly LicenseDbContext _context;
            private readonly ITenantProvider _tenantProvider;
            private readonly ILogger<LicenseRenewalService> _logger;

        public LicenseRenewalService(LicenseDbContext context, ITenantProvider tenantProvider, ILogger<LicenseRenewalService> logger)
            {
                _context = context;
                _tenantProvider = tenantProvider;
                _logger = logger;
            }

            public async Task RenewExpiredLicensesAsync()
            {
                try
                { 
                var expiredLicenses = await _context.LicenseApplications
                    .Where(l => l.TenantId == _tenantProvider.GetTenantId())
                    .Where(l => l.ExpiresAt < DateTime.UtcNow && l.Status == LicenseStatus.Approved)
                    .ToListAsync();

                foreach (var license in expiredLicenses)
                {
                    license.Status = LicenseStatus.Renewed;
                    license.ExpiresAt = DateTime.UtcNow.AddYears(1);
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("Renewed {Count} expired licenses", expiredLicenses.Count);
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "Error while renewing expired licenses");
                    throw; 
                }

            }
        }
    }


