using LicenseService.API.Application.Command;
using LicenseService.API.Data;
using LicenseService.API.Domain;
using LicenseService.API.Infrastructure.Tenant;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace LicenseService.API.Application.Handlers
{
    public class SubmitApplicationHandler : IRequestHandler<SubmitApplicationCommand, Guid>
    {
        private readonly LicenseDbContext _db;
        private readonly ITenantProvider _tenantProvider;

        public SubmitApplicationHandler(LicenseDbContext db, ITenantProvider tenantProvider)
        {
          _db = db;
          _tenantProvider = tenantProvider;
        }

        public async Task<Guid> Handle(SubmitApplicationCommand request, CancellationToken cancellationToken)
        {
            var app = new LicenseApplication
            {
                ApplicantId = request.ApplicantId,
                AgencyId = request.AgencyId,
                DataJson = request.DataJson,
                Status = LicenseStatus.Submitted,
                ExpiresAt = DateTime.UtcNow.AddYears(1),
                TenantId = _tenantProvider.GetTenantId()
            };

            _db.LicenseApplications.Add(app);
            await _db.SaveChangesAsync(cancellationToken);
                        
            return app.Id;
        }
    }

}
