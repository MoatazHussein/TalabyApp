using MediatR;
using Microsoft.Extensions.Logging;
using Talaby.Application.Features.Users.Services;
using Talaby.Domain.Exceptions;
using Talaby.Domain.Repositories.Projects;

namespace Talaby.Application.Features.Projects.ProjectRequests.Commands.DeleteProjectRequest
{
    internal class DeleteProjectRequestCommandHandler(ILogger<DeleteProjectRequestCommandHandler> logger,IUserContext userContext,
    IProjectRequestRepository projectRequestRepository) : IRequestHandler<DeleteProjectRequestCommand>
    {
        public async Task Handle(DeleteProjectRequestCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Deleting ProjectRequest with id: {ProjectRequestId}", request.Id);
            var projectRequest = await projectRequestRepository.GetByIdAsync(request.Id);
            if (projectRequest is null)
                throw new NotFoundException(nameof(projectRequest), request.Id.ToString());

            var currentUser = userContext.GetCurrentUser()
         ?? throw new UnAuthorizedAccessException("User not authenticated.");

            if (projectRequest.CreatorId != currentUser.Id)
                throw new BusinessRuleException("You are not allowed to delete this project.", 403);

            await projectRequestRepository.Delete(projectRequest);

        }
    }
}
