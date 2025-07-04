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

namespace Talaby.Application.Users.Commands.RegisterStore;

public class RegisterStoreCommandHandler : IRequestHandler<RegisterStoreCommand, Guid>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IStoreCategoryRepository _storeCategoryRepository;
    private readonly IMailService _mailService;
    private readonly IConfiguration _config;


    public RegisterStoreCommandHandler(UserManager<AppUser> userManager, IStoreCategoryRepository storeCategoryRepository, IMailService mailService, IConfiguration config)
    {
        _userManager = userManager;
        _storeCategoryRepository = storeCategoryRepository;
        _mailService = mailService;
        _config = config;
    }

    public async Task<Guid> Handle(RegisterStoreCommand request, CancellationToken cancellationToken)
    {
        var existingEmail = await _userManager.FindByEmailAsync(request.Email);
        if (existingEmail != null)
            throw new AlreadyExistsException("Email");


        var existingFirstName = await _userManager.Users
                                                  .AnyAsync(c => (c.FirstName == request.FirstName && c.UserType == UserType.Store), cancellationToken);

        if (existingFirstName)
            throw new AlreadyExistsException("First Name for Store");


        var existingCommercialRegisterNumber = await _userManager.Users
                                                     .AnyAsync(c => c.CommercialRegisterNumber == request.CommercialRegisterNumber,cancellationToken);
        
        if (existingCommercialRegisterNumber)
            throw new AlreadyExistsException("Commercial Register Number for Store");


        var existingStoreCategory = await _storeCategoryRepository.AnyAsync(e => e.Id == request.StoreCategoryId, cancellationToken);

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

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            throw new ApplicationException(string.Join(", ", result.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(user, UserRoles.Store);

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        var confirmUrl = $"{_config["App:ConfirmEmailApiUrl"]}?email={Uri.EscapeDataString(user.Email)}&token={Uri.EscapeDataString(token)}";

        var emailBody = $"<p>Please confirm your email by clicking <a href='{confirmUrl}'>here</a>.</p>";

        await _mailService.SendEmailAsync(user.Email, "Confirm your email", emailBody, null);



        return user.Id;
    }
}
