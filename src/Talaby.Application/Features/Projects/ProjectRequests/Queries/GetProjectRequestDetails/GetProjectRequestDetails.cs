using MediatR;

namespace Talaby.Application.Features.Projects.ProjectRequests.Queries.GetProjectRequestDetails;

public record GetProjectRequestDetailsQuery(Guid ProjectRequestId) : IRequest<ProjectRequestDetailsDto>;
