using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Talaby.Application.Common.Interfaces;
using Talaby.Domain.Constants;
using Talaby.Domain.Entities;
using Talaby.Domain.Enums;
using Talaby.Domain.Exceptions;
using Talaby.Domain.Repositories;

namespace Talaby.Application.Features.Users.Commands.RegisterStore;

public class RegisterStoreCommandHandler(UserManager<AppUser> userManager, IStoreCategoryRepository storeCategoryRepository, IMailService mailService
    , IConfiguration config) : IRequestHandler<RegisterStoreCommand, Guid>
{
    public async Task<Guid> Handle(RegisterStoreCommand request, CancellationToken cancellationToken)
    {
        var existingEmail = await userManager.FindByEmailAsync(request.Email);
        if (existingEmail != null)
            throw new AlreadyExistsException("Email");


        var existingFirstName = await userManager.Users
                                                  .AnyAsync(c => c.FirstName == request.FirstName && c.UserType == UserType.Store, cancellationToken);

        if (existingFirstName)
            throw new AlreadyExistsException("First Name for Store");


        var existingCommercialRegisterNumber = await userManager.Users
                                                     .AnyAsync(c => c.CommercialRegisterNumber == request.CommercialRegisterNumber,cancellationToken);
        
        if (existingCommercialRegisterNumber)
            throw new AlreadyExistsException("Commercial Register Number for Store");


        var existingStoreCategory = await storeCategoryRepository.AnyAsync(e => e.Id == request.StoreCategoryId, cancellationToken);

        if (!existingStoreCategory)
            throw new NotFoundException(nameof(StoreCategory), request.StoreCategoryId.ToString());


        var user = new AppUser
        {
            FirstName = request.FirstName,
            //LastName = request.LastName,
            UserName = request.Email,
            Email = request.Email,
            Mobile = request.Mobile,
            Location = request.Location,
            CommercialRegisterNumber = request.CommercialRegisterNumber,
            CommercialRegisterImageUrl = request.CommercialRegisterImageUrl,
            StoreCategoryId = request.StoreCategoryId,
            UserType = UserType.Store
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            throw new ApplicationException(string.Join(", ", result.Errors.Select(e => e.Description)));

        await userManager.AddToRoleAsync(user, UserRoles.Store);

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

        var confirmUrl = $"{config["App:ConfirmEmailApiUrl"]}?email={Uri.EscapeDataString(user.Email)}&token={Uri.EscapeDataString(token)}";

        var emailBody = $"<p>Please confirm your email by clicking <a href='{confirmUrl}'>here</a>.</p>";

        await mailService.SendEmailAsync(user.Email, "Confirm your email", emailBody, null);



        return user.Id;
    }
}
