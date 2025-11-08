namespace LicenseService.API.Application.Interfaces
{
    public interface IUserContext
    {
        string? UserId { get; }
        Guid TenantId { get; }
    }
}
