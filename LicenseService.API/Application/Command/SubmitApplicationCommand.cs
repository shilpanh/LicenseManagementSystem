using MediatR;
namespace LicenseService.API.Application.Command
{
    public record SubmitApplicationCommand(string ApplicantId, string AgencyId, string DataJson) : IRequest<Guid>;

}
