using MediatR;

namespace Talaby.Application.Projects.ProjectRequests.Queries.GetProjectRequestDetails;

public record GetProjectRequestDetailsQuery(Guid ProjectRequestId) : IRequest<ProjectRequestDetailsDto>;
