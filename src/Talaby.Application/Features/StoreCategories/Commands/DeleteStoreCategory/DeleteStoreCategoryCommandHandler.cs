using MediatR;
using Microsoft.Extensions.Logging;
using Talaby.Domain.Entities;
using Talaby.Domain.Exceptions;
using Talaby.Domain.Repositories;

namespace Talaby.Application.Features.StoreCategories.Commands.DeleteStoreCategory
{
    internal class DeleteStoreCategoryCommandHandler(ILogger<DeleteStoreCategoryCommandHandler> logger,
    IStoreCategoryRepository storeCategoryRepository) : IRequestHandler<DeleteStoreCategoryCommand>
    {
        public async Task Handle(DeleteStoreCategoryCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Deleting store Category with id: {storeCategoryId}", request.Id);
            var storeCategory = await storeCategoryRepository.GetByIdAsync(request.Id);
            if (storeCategory is null)
                throw new NotFoundException(nameof(StoreCategory), request.Id.ToString());

            await storeCategoryRepository.Delete(storeCategory);

        }
    }
}
