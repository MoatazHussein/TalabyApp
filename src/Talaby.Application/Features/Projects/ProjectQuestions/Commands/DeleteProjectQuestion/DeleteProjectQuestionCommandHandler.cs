using MediatR;
using Microsoft.Extensions.Logging;
using Talaby.Application.Features.Users.Services;
using Talaby.Domain.Entities.Projects;
using Talaby.Domain.Exceptions;
using Talaby.Domain.Repositories.Projects;

namespace Talaby.Application.Features.Projects.ProjectQuestions.Commands.DeleteProjectQuestion
{
    internal class DeleteProjectQuestionCommandHandler(ILogger<DeleteProjectQuestionCommandHandler> logger,IUserContext userContext,
    IProjectQuestionRepository projectQuestionRepository) : IRequestHandler<DeleteProjectQuestionCommand>
    {
        public async Task Handle(DeleteProjectQuestionCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Deleting ProjectQuestion with id: {ProjectQuestionId}", request.Id);
            var projectQuestion = await projectQuestionRepository.GetByIdAsync(request.Id);
            if (projectQuestion is null)
                throw new NotFoundException(nameof(ProjectQuestion), request.Id.ToString());

            var currentUser = userContext.GetCurrentUser()
                  ?? throw new UnAuthorizedAccessException("User not authenticated.");

            if (projectQuestion.CreatorId != currentUser.Id)
                throw new BusinessRuleException("You are not allowed to delete this Question.", 403);

            await projectQuestionRepository.Delete(projectQuestion);

        }
    }
}
