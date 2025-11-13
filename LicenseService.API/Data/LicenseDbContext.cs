using LicenseService.API.Domain;
using LicenseService.API.Domain.Entity;
using LicenseService.API.Infrastructure.Tenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
namespace LicenseService.API.Data
{
    public class LicenseDbContext : DbContext
    {
        private readonly ITenantProvider _tenantProvider;

        public LicenseDbContext(DbContextOptions<LicenseDbContext> options, ITenantProvider tenantProvider)
            : base(options)
        {
            _tenantProvider = tenantProvider;
        }

        public DbSet<LicenseApplication> LicenseApplications { get; set; }
        public DbSet<User> Users { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LicenseApplication>().HasQueryFilter(a => a.TenantId == _tenantProvider.GetTenantId());
            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            SetTenantIdForNewEntities();
            return base.SaveChanges();
        }

        private void SetTenantIdForNewEntities()
        {
            var entries = ChangeTracker.Entries().Where(e => e.State == EntityState.Added);
            foreach (var entry in entries)
            {
                if (entry.Entity is ITenantEntity tenantEntity)
                {
                    tenantEntity.TenantId = _tenantProvider.GetTenantId();
                }
            }
        }
    }

}
