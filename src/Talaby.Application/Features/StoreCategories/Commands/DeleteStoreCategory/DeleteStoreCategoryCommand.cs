using MediatR;

namespace Talaby.Application.Features.StoreCategories.Commands.DeleteStoreCategory;

public class DeleteStoreCategoryCommand(int id) : IRequest
{
    public int Id { get; } = id;
}
