using AutoMapper;
using Talaby.Application.Features.Projects.ProjectQuestions.Commands.UpdateProjectQuestion;
using Talaby.Domain.Entities.Projects;

namespace Talaby.Application.Common.Mappings;

public class ProjectQuestionMappingProfile : Profile
{
    public ProjectQuestionMappingProfile()
    {
        CreateMap<UpdateProjectQuestionCommand, ProjectQuestion>();

    }
}
