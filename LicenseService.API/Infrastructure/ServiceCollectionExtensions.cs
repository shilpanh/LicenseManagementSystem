using LicenseService.API.Data;
using LicenseService.API.Infrastructure.Tenant;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace LicenseService.API.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<ITenantProvider, HeaderTenantProvider>();

            services.AddDbContext<LicenseDbContext>(opts =>
                opts.UseSqlite(config.GetConnectionString("DefaultConnection")));

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(typeof(LicenseService.API.Application.AssemblyMarker));
            return services;
        }
    }

}
