using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Talaby.Domain.Entities;
using Talaby.Domain.Enums;
using Talaby.Domain.Exceptions;
using Talaby.Domain.Repositories;

namespace Talaby.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler(UserManager<AppUser> userManager, IStoreCategoryRepository storeCategoryRepository, IMapper mapper) : IRequestHandler<UpdateUserCommand, bool>
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly IStoreCategoryRepository _storeCategoryRepository = storeCategoryRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users
            .Include(u => u.StoreCategory)
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

        if (user == null)
            throw new NotFoundException(nameof(user), request.Id.ToString());


        // store checks 
        if (user.UserType == UserType.Store)
        {
            var existingFirstName = await _userManager.Users
                                    .AnyAsync(c => c.FirstName == request.FirstName && c.Id != request.Id, cancellationToken);

            if (existingFirstName)
                throw new AlreadyExistsException("First Name for Store");


            var existingCommercialRegisterNumber = await _userManager.Users
                                                   .AnyAsync(c => c.CommercialRegisterNumber == request.CommercialRegisterNumber && c.Id != request.Id, cancellationToken);

            if (existingCommercialRegisterNumber)
                throw new AlreadyExistsException("Commercial Register Number for Store");


            var existingStoreCategory = await _storeCategoryRepository.AnyAsync(e => e.Id == request.StoreCategoryId, cancellationToken);

            if (!existingStoreCategory)
                throw new NotFoundException(nameof(StoreCategory), request.StoreCategoryId.ToString()!);
        }



        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Mobile = request.Mobile;
        user.Location = request.Location;
        user.CommercialRegisterNumber = request.CommercialRegisterNumber;
        user.CommercialRegisterImageUrl = request.CommercialRegisterImageUrl;
        user.StoreCategoryId = request.StoreCategoryId;

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            throw new AppException("Failed to update user: " + string.Join(", ", updateResult.Errors.Select(e => e.Description)));

        return true;
    }
}
