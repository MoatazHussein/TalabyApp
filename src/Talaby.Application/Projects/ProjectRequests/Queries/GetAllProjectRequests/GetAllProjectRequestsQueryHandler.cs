using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Talaby.Application.Common;
using Talaby.Application.Projects.ProjectRequests.Queries.Dtos;
using Talaby.Application.Users.Dtos;
using Talaby.Domain.Entities.Projects;
using Talaby.Domain.Repositories.Projects;

namespace Talaby.Application.Projects.ProjectRequests.Queries.GetAllProjectRequests;

public class GetAllProjectRequestsQueryHandler(ILogger<GetAllProjectRequestsQuery> logger,
    IMapper mapper,
    IProjectRequestRepository ProjectRequestRepository) : IRequestHandler<GetAllProjectRequestsQuery, PagedResult<ProjectRequestDto>>
{
    public async Task<PagedResult<ProjectRequestDto>> Handle(GetAllProjectRequestsQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting all Project Requests");
        var (projectRequests, totalCount) = await ProjectRequestRepository.GetAllMatchingAsync(request.SearchPhrase,
            request.PageSize,
            request.PageNumber,
            request.SortBy,
            request.SortDirection);

        var projectRequestDtos = mapper.Map<List<ProjectRequestDto>>(projectRequests);

        var result = new PagedResult<ProjectRequestDto>(projectRequestDtos, totalCount, request.PageSize, request.PageNumber);
        return result;
    }

 
}