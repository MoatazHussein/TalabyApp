using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Talaby.Domain.Exceptions;
using Talaby.Domain.Repositories;

namespace Talaby.Application.StoreCategories.Commands.UpdateStoreCategory;

public class UpdateStoreCategoryCommandHandler(ILogger<UpdateStoreCategoryCommandHandler> logger,
    IStoreCategoryRepository storeCategoryRepository, IMapper mapper) : IRequestHandler<UpdateStoreCategoryCommand>
{
    public async Task Handle(UpdateStoreCategoryCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating StoreCategory with id: {StoreCategoryId} with {@UpdatedStoreCategory}", request.Id, request);
        var StoreCategory = await storeCategoryRepository.GetByIdAsync(request.Id);
        if (StoreCategory is null)
            throw new NotFoundException(nameof(StoreCategory), request.Id.ToString());

        var existingStoreCategory = await storeCategoryRepository.AnyAsync(
            e => ((e.NameEn == request.NameEn || e.NameAr == request.NameAr) && e.Id != request.Id)  , cancellationToken);

        if (existingStoreCategory)
        {
            throw new AlreadyExistsException($"NameEn:{request.NameEn} Or NameAr:{request.NameAr}");
        }


        mapper.Map(request, StoreCategory);
        await storeCategoryRepository.SaveChanges();
    }
}
