using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Talaby.Application.Features.Projects.ProjectRequests.Queries.Dtos;
using Talaby.Domain.Entities.Projects;
using Talaby.Domain.Exceptions;
using Talaby.Domain.Repositories.Projects;

namespace Talaby.Application.Features.Projects.ProjectRequests.Queries.GetProjectRequestById;

public class GetProjectRequestByIdQueryHandler(IProjectRequestRepository projectRequestRepository, 
    ILogger<GetProjectRequestByIdQueryHandler> logger, IMapper mapper) : IRequestHandler<GetProjectRequestByIdQuery, ProjectRequestDto>
{
    public async Task<ProjectRequestDto?> Handle(GetProjectRequestByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting ProjectRequest with Id {ProjectRequestId}", request.Id);

        var existingProjectRequest = await projectRequestRepository.GetByIdAsync(request.Id, pr => pr.StoreCategory);

        if (existingProjectRequest==null)
        {
            throw new NotFoundException(nameof(ProjectRequest), request.Id.ToString());
        }

        var projectRequestDto = mapper.Map<ProjectRequestDto>(existingProjectRequest);

        return projectRequestDto;
    }
}
