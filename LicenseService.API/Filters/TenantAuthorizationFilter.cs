using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using LicenseService.API.Application.Interfaces;
namespace LicenseService.API.Filters
{  
    public class TenantAuthorizationFilter : IActionFilter
    {
        private readonly IUserContext _userContext;

        public TenantAuthorizationFilter(IUserContext userContext)
        {
            _userContext = userContext;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (_userContext.TenantId == Guid.Empty)
            {
                context.Result = new BadRequestObjectResult("TenantId is missing or invalid.");
            }

            if (string.IsNullOrEmpty(_userContext.UserId))
            {
                context.Result = new UnauthorizedObjectResult("User identity missing.");
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }

}
