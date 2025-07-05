using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Talaby.Domain.Exceptions;
using Talaby.Domain.Repositories.Projects;

namespace Talaby.Application.Projects.ProjectRequests.Commands.UpdateProjectRequest;

public class UpdateProjectRequestCommandHandler(ILogger<UpdateProjectRequestCommandHandler> logger,
    IProjectRequestRepository projectRequestRepository, IMapper mapper) : IRequestHandler<UpdateProjectRequestCommand>
{
    public async Task Handle(UpdateProjectRequestCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating ProjectRequest with id: {ProjectRequestId} with {@UpdatedProjectRequest}", request.Id, request);
        var ProjectRequest = await projectRequestRepository.GetByIdAsync(request.Id);
        if (ProjectRequest is null)
            throw new NotFoundException(nameof(ProjectRequest), request.Id.ToString());

        var existingProjectRequest = await projectRequestRepository.AnyAsync( p => p.Id == request.Id  , cancellationToken);

        if (!existingProjectRequest)
        {
            throw new NotFoundException(nameof(ProjectRequest), request.Id.ToString());
        }

        mapper.Map(request, ProjectRequest);

        await projectRequestRepository.SaveChanges();
    }
}
