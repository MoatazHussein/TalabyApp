using MediatR;
using Talaby.Application.Common;
using Talaby.Domain.Constants;
using Talaby.Domain.Entities;

namespace Talaby.Application.StoreCategories.Queries.GetAllStoreCategories;

public class GetAllStoreCategoriesQuery : IRequest<PagedResult<StoreCategory>>
{
    public string? SearchPhrase { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string? SortBy { get; set; }
    public SortDirection SortDirection { get; set; }
}