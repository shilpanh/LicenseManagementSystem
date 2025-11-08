using Hangfire.Dashboard;
namespace LicenseService.API.Filters
{
    public class HangfireAllowAllFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            // WARNING: This allows anyone to access the Hangfire dashboard.
            // Use only for local development or testing!
            return true;
        }
    }

}
