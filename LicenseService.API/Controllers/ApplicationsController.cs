using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using LicenseService.API.Application.Command;
using LicenseService.API.Data;
using LicenseService.API.Application.Interfaces;
using LicenseService.API.Filters;


namespace LicenseService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(TenantAuthorizationFilter))]
    public class ApplicationsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ApplicationsController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        [Authorize(Roles = "Applicant")]
        public async Task<IActionResult> Submit([FromBody] SubmitDto dto, [FromServices] IUserContext userContext)
        {
            var cmd = new SubmitApplicationCommand(userContext.UserId!, dto.AgencyId, dto.DataJson);
            var id = await _mediator.Send(cmd);
            return CreatedAtAction(nameof(Get), new { id }, new { id });
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get(Guid id, [FromServices] LicenseDbContext db)
        {
            var app = await db.LicenseApplications.FindAsync(id);
            if (app is null) return NotFound();
            return Ok(app);
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetAll([FromServices] LicenseDbContext db)
        {
            var apps = db.LicenseApplications.ToList();
            return Ok(apps);
        }

    }

    public class SubmitDto
    {
        public string AgencyId { get; set; }
        public string DataJson { get; set; }
    }

}
