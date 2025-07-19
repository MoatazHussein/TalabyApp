using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Talaby.Application.Common.Interfaces;
using Talaby.Domain.Entities;
using Talaby.Domain.Exceptions;
using Talaby.Domain.Repositories;

namespace Talaby.Application.Features.StoreCategories.Queries.GetStoreCategoryById;

public class GetStoreCategoryByIdQueryHandler(IStoreCategoryRepository StoreCategoriesRepository, 
    ILogger<GetStoreCategoryByIdQueryHandler> logger, IMapper mapper, ITimeZoneConverter timeZoneConverter) : IRequestHandler<GetStoreCategoryByIdQuery, StoreCategory>
{
    public async Task<StoreCategory?> Handle(GetStoreCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting storeCategory with Id {storeCategoryId}", request.Id);

        var storeCategory = await StoreCategoriesRepository.GetByIdAsync(request.Id)
                        ?? throw new NotFoundException(nameof(StoreCategory), request.Id.ToString());

        var storeCategoryDto = mapper.Map<StoreCategory>(storeCategory);

        return timeZoneConverter.ConvertUtcToLocal(storeCategoryDto);
    }
}
