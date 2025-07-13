using MediatR;
using Talaby.Domain.Entities;

namespace Talaby.Application.Features.StoreCategories.Queries.GetStoreCategoryById;

public class GetStoreCategoryByIdQuery(int id) : IRequest<StoreCategory>
{
    public int Id { get;} = id;
}
