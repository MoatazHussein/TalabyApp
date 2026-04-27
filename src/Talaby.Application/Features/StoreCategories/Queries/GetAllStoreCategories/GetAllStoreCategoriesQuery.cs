using MediatR;
using Talaby.Application.Common;
using Talaby.Domain.Constants;
using Talaby.Domain.Entities;

namespace Talaby.Application.Features.StoreCategories.Queries.GetAllStoreCategories;

public class GetAllStoreCategoriesQuery : IRequest<PagedResult<StoreCategory>>
{
    public string? SearchPhrase { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }
    public SortDirection? SortDirection { get; set; }
}
