using MediatR;
using Microsoft.Extensions.Logging;
using Talaby.Application.Common.Interfaces;
using Talaby.Application.Features.Users.Services;
using Talaby.Domain.Constants;
using Talaby.Domain.Enums;
using Talaby.Domain.Exceptions;
using Talaby.Domain.Repositories.Payments;
using Talaby.Domain.Repositories.Projects;

namespace Talaby.Application.Features.Payments.Queries.VerifyProjectCommissionPayment;

public sealed class VerifyProjectCommissionPaymentQueryHandler(
    IProjectRequestRepository projectRequestRepository,
    IProjectCommissionPaymentRepository commissionPaymentRepository,
    IUserContext userContext,
    ITimeZoneConverter timeZoneConverter,
    ILogger<VerifyProjectCommissionPaymentQueryHandler> logger)
    : IRequestHandler<VerifyProjectCommissionPaymentQuery, VerifyProjectCommissionPaymentResponse>
{
    public async Task<VerifyProjectCommissionPaymentResponse> Handle(
        VerifyProjectCommissionPaymentQuery request,
        CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser()
            ?? throw new UnAuthorizedAccessException("User is not authenticated.");

        var projectRequest = await projectRequestRepository.GetByIdAsync(request.ProjectRequestId)
            ?? throw new NotFoundException("ProjectRequest", request.ProjectRequestId.ToString());

        var isOwner = projectRequest.CreatorId == currentUser.Id;
        var isAdmin = currentUser.IsInRole(UserRoles.Admin);

        if (!isOwner && !isAdmin)
            throw new UnAuthorizedAccessException("Access denied. You are not the owner of this project request.");

        var commissionPayment = await commissionPaymentRepository
            .GetByProjectRequestIdAsync(request.ProjectRequestId, cancellationToken);

        logger.LogDebug(
            "Verify commission payment. ProjectRequestId={ProjectRequestId}, UserId={UserId}, ProjectStatus={ProjectStatus}, PaymentStatus={PaymentStatus}",
            request.ProjectRequestId, currentUser.Id, projectRequest.Status, commissionPayment?.Status);

        var result = new VerifyProjectCommissionPaymentResponse(
            ProjectRequestId: request.ProjectRequestId,
            ProjectStatus: projectRequest.Status,
            PaymentStatus: commissionPayment?.Status,
            IsPaid: commissionPayment?.Status == ProjectCommissionPaymentStatus.Paid,
            PaidAt: commissionPayment?.PaidAtUtc);

        return timeZoneConverter.ConvertUtcToLocal(result);
    }
}
