using MediatR;
using Microsoft.Extensions.Logging;
using Talaby.Application.Common.Interfaces;
using Talaby.Application.Features.Users.Services;
using Talaby.Domain.Entities.Projects;
using Talaby.Domain.Enums;
using Talaby.Domain.Exceptions;
using Talaby.Domain.Repositories.Projects;

namespace Talaby.Application.Features.Projects.ProjectRequests.Commands.CreateProjectRequest;

public class CreateProjectRequestCommandHandler(
    IProjectRequestRepository repository,
    IUserContext userContext,
    IUserConfirmationGuard userConfirmationGuard,
    IUserActionGuard userActionGuard,
    ILogger<CreateProjectRequestCommandHandler> logger,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateProjectRequestCommand, Guid>
{
    public async Task<Guid> Handle(CreateProjectRequestCommand request, CancellationToken cancellationToken)
    {
        await userConfirmationGuard.EnsureCurrentUserEmailConfirmedAsync(cancellationToken);

        var currentUser = userContext.GetCurrentUser();

        await userActionGuard.EnsureCanCreateProjectAsync(currentUser.Id, cancellationToken);

        // Block creation if the client has an unpaid project awaiting commission payment.
        var hasUnpaidProject = await repository.AnyAsync(
            r => r.CreatorId == currentUser.Id
                 && r.Status == ProjectRequestStatus.AwaitingCommissionPayment,
            cancellationToken);

        if (hasUnpaidProject)
        {
            logger.LogWarning(
                "Project request creation blocked. UserId={UserId} has an unpaid commission payment.",
                currentUser.Id);

            throw new BusinessRuleException(
                "You have a project awaiting commission payment. Please complete the payment before creating a new request.",
                409);
        }

        var entity = new ProjectRequest
        {
            Title = request.Title,
            Description = request.Description,
            ImageUrl = request.ImageUrl,
            MinBudget = request.MinBudget,
            MaxBudget = request.MaxBudget,
            StoreCategoryId = request.StoreCategoryId,
            CreatorId = currentUser.Id,
            CreatedAt = DateTime.UtcNow
        };

        await repository.Create(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
