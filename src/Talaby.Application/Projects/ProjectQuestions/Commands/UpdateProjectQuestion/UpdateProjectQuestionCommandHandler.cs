using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Talaby.Domain.Entities.Projects;
using Talaby.Domain.Exceptions;
using Talaby.Domain.Repositories.Projects;

namespace Talaby.Application.Projects.ProjectQuestions.Commands.UpdateProjectQuestion;

public class UpdateProjectQuestionCommandHandler(ILogger<UpdateProjectQuestionCommandHandler> logger,
    IProjectQuestionRepository projectQuestionRepository, IMapper mapper) : IRequestHandler<UpdateProjectQuestionCommand>
{
    public async Task Handle(UpdateProjectQuestionCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating ProjectQuestion with id: {ProjectQuestionId} with {@UpdatedProjectQuestion}", request.Id, request);
        var projectQuestion = await projectQuestionRepository.GetByIdAsync(request.Id);
        if (projectQuestion is null)
            throw new NotFoundException(nameof(projectQuestion), request.Id.ToString());

        var existingProjectQuestion = await projectQuestionRepository.AnyAsync( p => p.Id == request.Id  , cancellationToken);

        if (!existingProjectQuestion)
        {
            throw new NotFoundException(nameof(ProjectQuestion), request.Id.ToString());
        }

        mapper.Map(request, projectQuestion);

        await projectQuestionRepository.SaveChanges();
    }
}
