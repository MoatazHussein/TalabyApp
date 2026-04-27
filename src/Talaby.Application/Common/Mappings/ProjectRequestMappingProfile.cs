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
        CreateMap<ProjectRequest, ProjectRequestDto>();

        CreateMap<UpdateProjectRequestCommand, ProjectRequest>();

    }
}
