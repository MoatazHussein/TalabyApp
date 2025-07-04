using MediatR;

namespace Talaby.Application.StoreCategories.Commands.DeleteStoreCategory;

public class DeleteStoreCategoryCommand(int id) : IRequest
{
    public int Id { get; } = id;
}
