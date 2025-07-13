using MediatR;
using Talaby.Domain.Entities.Projects;
using Talaby.Domain.Exceptions;

namespace Talaby.Application.Features.Projects.ProjectRequests.Queries.GetProjectRequestDetails;

public class GetProjectRequestDetailsHandler(IProjectRequestDetailsRepository repository) : IRequestHandler<GetProjectRequestDetailsQuery, ProjectRequestDetailsDto>
{
    public async Task<ProjectRequestDetailsDto> Handle(GetProjectRequestDetailsQuery request, CancellationToken cancellationToken)
    {
        var result = await repository.GetDetailsAsync(request.ProjectRequestId, cancellationToken);

        if (result == null)
            throw new NotFoundException(nameof(ProjectRequest), request.ProjectRequestId.ToString());

        return result;
    }
}
