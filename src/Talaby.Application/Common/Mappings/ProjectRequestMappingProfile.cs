using AutoMapper;
using Talaby.Application.Features.Projects.ProjectRequests.Commands.UpdateProjectRequest;
using Talaby.Application.Features.Projects.ProjectRequests.Queries.Dtos;
using Talaby.Application.Features.Users.Dtos;
using Talaby.Domain.Entities;
using Talaby.Domain.Entities.Projects;

namespace Talaby.Application.Common.Mappings;

public class ProjectRequestMappingProfile : Profile
{
    public ProjectRequestMappingProfile()
    {
        CreateMap<ProjectRequest, ProjectRequestDto>()
            .ForMember(dest => dest.StatusValue, opt => opt.MapFrom(src => (int)src.Status))
            .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<UpdateProjectRequestCommand, ProjectRequest>();

    }
}
