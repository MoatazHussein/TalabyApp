namespace Talaby.Application.Projects.ProjectRequests.Queries.GetProjectRequestDetails
{
    public interface IProjectRequestDetailsRepository
    {
        Task<ProjectRequestDetailsDto?> GetDetailsAsync(Guid requestId, CancellationToken cancellationToken);

    }
}
