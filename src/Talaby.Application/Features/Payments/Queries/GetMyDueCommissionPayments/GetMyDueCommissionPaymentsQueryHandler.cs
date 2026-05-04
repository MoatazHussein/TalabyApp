using MediatR;
using Talaby.Application.Common.Interfaces;
using Talaby.Application.Features.Users.Services;
using Talaby.Domain.Constants;
using Talaby.Domain.Exceptions;

namespace Talaby.Application.Features.Payments.Queries.GetMyDueCommissionPayments;

public class GetMyDueCommissionPaymentsQueryHandler(
    IUserContext userContext,
    ICommissionPaymentReadRepository repository,
    ITimeZoneConverter timeZoneConverter)
    : IRequestHandler<GetMyDueCommissionPaymentsQuery, IReadOnlyList<DueCommissionPaymentDto>>
{
    public async Task<IReadOnlyList<DueCommissionPaymentDto>> Handle(
        GetMyDueCommissionPaymentsQuery request,
        CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        if (!currentUser.IsInRole(UserRoles.Client))
            throw new BusinessRuleException("Only client users can view their due commission payments.", 403);

        var result = await repository.GetDueCommissionPaymentsForClientAsync(
            currentUser.Id,
            cancellationToken);

        return timeZoneConverter.ConvertUtcToLocal(result);
    }
}
