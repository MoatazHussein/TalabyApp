using MediatR;
using Talaby.Application.Common.Interfaces;
using Talaby.Application.Features.Users.Services;

namespace Talaby.Application.Features.Users.PolicyViolations.Commands.ReviewPolicyViolation;

public sealed class ReviewPolicyViolationCommandHandler(
    IUserContext userContext,
    IUserPolicyViolationService userPolicyViolationService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<ReviewPolicyViolationCommand>
{
    public async Task Handle(
        ReviewPolicyViolationCommand request,
        CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        await userPolicyViolationService.ReviewViolationAsync(
            request.ViolationId,
            request.ReviewStatus,
            request.ReviewNote,
            currentUser.Id,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
