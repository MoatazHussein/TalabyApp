using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Talaby.Application.Common;
using Talaby.Domain.Entities;
using Talaby.Domain.Repositories;

namespace Talaby.Application.StoreCategories.Queries.GetAllStoreCategories;

public class GetAllStoreCategoriesQueryHandler(ILogger<GetAllStoreCategoriesQuery> logger,
    IMapper mapper,
    IStoreCategoryRepository StoreCategoryCategoriesRepository) : IRequestHandler<GetAllStoreCategoriesQuery, PagedResult<StoreCategory>>
{
    public async Task<PagedResult<StoreCategory>> Handle(GetAllStoreCategoriesQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting all Categories");
        var (Categories, totalCount) = await StoreCategoryCategoriesRepository.GetAllMatchingAsync(request.SearchPhrase,
            request.PageSize,
            request.PageNumber,
            request.SortBy,
            request.SortDirection);


        var result = new PagedResult<StoreCategory>(Categories, totalCount, request.PageSize, request.PageNumber);
        return result;
    }

 
}