using AutoMapper;
using Talaby.Application.Projects.ProjectProposals.Commands.UpdateProjectProposal;
using Talaby.Domain.Entities.Projects;

namespace Talaby.Application.Common.Mappings;

public class ProjectProposalMappingProfile : Profile
{
    public ProjectProposalMappingProfile()
    {
        CreateMap<UpdateProjectProposalCommand, ProjectProposal>();

    }
}
