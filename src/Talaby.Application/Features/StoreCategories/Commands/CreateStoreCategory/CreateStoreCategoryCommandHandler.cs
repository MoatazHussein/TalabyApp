using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Talaby.Application.Common.Interfaces;
using Talaby.Application.Features.Users.Services;
using Talaby.Domain.Entities;
using Talaby.Domain.Exceptions;
using Talaby.Domain.Repositories;

namespace Talaby.Application.Features.StoreCategories.Commands.CreateStoreCategory;

public class CreateStoreCategoryCommandHandler(ILogger<CreateStoreCategoryCommandHandler> logger, IMapper mapper, IStoreCategoryRepository storeCategoryRepository
     , IUserContext userContext, IUnitOfWork unitOfWork) : IRequestHandler<CreateStoreCategoryCommand, int>
{
    public async Task<int> Handle(CreateStoreCategoryCommand request, CancellationToken cancellationToken)
    {


        var existingStoreCategory = await storeCategoryRepository.AnyAsync(e => e.NameEn == request.NameEn || e.NameAr == request.NameAr, cancellationToken);

        if (existingStoreCategory)
        {
            throw new AlreadyExistsException($"NameEn:{request.NameEn} Or NameAr:{request.NameAr}");
        }


        var storeCategory = mapper.Map<StoreCategory>(request);


        await storeCategoryRepository.Create(storeCategory);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return storeCategory.Id;
    }
}
