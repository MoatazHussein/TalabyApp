using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Talaby.Application.Common;
using Talaby.Application.Common.Interfaces;
using Talaby.Application.Features.Projects.ProjectRequests.Queries.Dtos;
using Talaby.Domain.Repositories.Projects;

namespace Talaby.Application.Features.Projects.ProjectRequests.Queries.GetAllProjectRequests;

public class GetAllProjectRequestsQueryHandler(ILogger<GetAllProjectRequestsQuery> logger,
    IMapper mapper,
    IProjectRequestRepository ProjectRequestRepository,
    ITimeZoneConverter timeZoneConverter)
    : IRequestHandler<GetAllProjectRequestsQuery, PagedResult<ProjectRequestDto>>
{
    public async Task<PagedResult<ProjectRequestDto>> Handle(GetAllProjectRequestsQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting all Project Requests");
        var (projectRequests, totalCount) = await ProjectRequestRepository.GetAllMatchingAsync(request.StoreCategoryId, request.SearchPhrase,
            request.PageSize,
            request.PageNumber,
            request.SortBy,
            request.SortDirection);

        var projectRequestDtos = mapper.Map<List<ProjectRequestDto>>(projectRequests);

        var result = new PagedResult<ProjectRequestDto>(projectRequestDtos, totalCount, request.PageSize, request.PageNumber);

        //return result;
        return timeZoneConverter.ConvertUtcToLocal(result);
    }
}