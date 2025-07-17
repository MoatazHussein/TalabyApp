using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Talaby.Application.Features.Users;
using Talaby.Domain.Entities.Projects;
using Talaby.Domain.Exceptions;
using Talaby.Domain.Repositories.Projects;

namespace Talaby.Application.Features.Projects.ProjectQuestions.Commands.UpdateProjectQuestion;

public class UpdateProjectQuestionCommandHandler(ILogger<UpdateProjectQuestionCommandHandler> logger,IUserContext userContext,
    IProjectQuestionRepository projectQuestionRepository, IMapper mapper) : IRequestHandler<UpdateProjectQuestionCommand>
{
    public async Task Handle(UpdateProjectQuestionCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating ProjectQuestion with id: {ProjectQuestionId} with {@UpdatedProjectQuestion}", request.Id, request);
        var projectQuestion = await projectQuestionRepository.GetByIdAsync(request.Id);
        if (projectQuestion is null)
            throw new NotFoundException(nameof(projectQuestion), request.Id.ToString());

        var currentUser = userContext.GetCurrentUser()
               ?? throw new UnAuthorizedAccessException("User not authenticated.");

        if (projectQuestion.CreatorId != currentUser.Id)
            throw new BusinessRuleException("You are not allowed to update this Question.", 403);

        mapper.Map(request, projectQuestion);

        await projectQuestionRepository.SaveChanges();
    }
}
