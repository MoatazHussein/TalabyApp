using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Talaby.Application.Features.Users;
using Talaby.Domain.Entities;
using Talaby.Domain.Exceptions;
using Talaby.Domain.Repositories;

namespace Talaby.Application.Features.StoreCategories.Commands.CreateStoreCategory;

public class CreateStoreCategoryCommandHandler(ILogger<CreateStoreCategoryCommandHandler> logger, IMapper mapper, IStoreCategoryRepository storeCategoryRepository
     , IUserContext userContext) : IRequestHandler<CreateStoreCategoryCommand, int>
{
    public async Task<int> Handle(CreateStoreCategoryCommand request, CancellationToken cancellationToken)
    {
        //var currentUser = userContext.GetCurrentUser();

        //logger.LogInformation("{UserEmail} [{UserId}] is creating a new storeCategory {@StoreCategory}",
        //           currentUser.Email,
        //           currentUser.Id,
        //           request);


        var existingStoreCategory = await storeCategoryRepository.AnyAsync(e => e.NameEn == request.NameEn || e.NameAr == request.NameAr, cancellationToken);

        if (existingStoreCategory)
        {
            throw new AlreadyExistsException($"NameEn:{request.NameEn} Or NameAr:{request.NameAr}");
        }


        var storeCategory = mapper.Map<StoreCategory>(request);


        int id = await storeCategoryRepository.Create(storeCategory);

        return id;
    }
}
