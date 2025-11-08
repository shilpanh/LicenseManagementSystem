using System;
namespace LicenseService.API.Domain
{
    public enum LicenseStatus { Draft, Submitted, UnderReview, Approved, Rejected, Renewed }

    public class LicenseApplication : ITenantEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string TenantId { get; set; }
        public string ApplicantId { get; set; } // user id
        public string AgencyId { get; set; } // which agency
        public string DataJson { get; set; } // flexible payload
        public LicenseStatus Status { get; set; } = LicenseStatus.Draft;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiresAt { get; set; }
    }

}
