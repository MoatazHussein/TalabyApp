using MediatR;
using Talaby.Application.Common;
using Talaby.Application.Common.Interfaces;
using Talaby.Application.Features.Projects.ProjectProposals.Queries.ProposalsByProjectRequestId;
using Talaby.Application.Features.Users.Services;
using Talaby.Domain.Constants;
using Talaby.Domain.Exceptions;

namespace Talaby.Application.Features.Projects.ProjectProposals.Queries.MyProjectProposals;

public class GetMyProjectProposalsQueryHandler(
    IUserContext userContext,
    IProjectProposalReadRepository repository,
    ICommercialRegisterNumberMasker commercialRegisterNumberMasker,
    ITimeZoneConverter timeZoneConverter)
    : IRequestHandler<GetMyProjectProposalsQuery, PagedResult<MyProjectProposalListItemDto>>
{
    public async Task<PagedResult<MyProjectProposalListItemDto>> Handle(
        GetMyProjectProposalsQuery request,
        CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        if (!currentUser.IsInRole(UserRoles.Store))
            throw new BusinessRuleException("Only store users can view their project proposals.", 403);

        var result = await repository.GetPagedUserProposalsAsync(
            currentUser.Id,
            request.SearchPhrase,
            request.PageNumber,
            request.PageSize,
            request.SortBy,
            request.SortDirection,
            cancellationToken);

        foreach (var proposal in result.Items)
        {
            proposal.CreatorCommercialRegisterNumber =
                commercialRegisterNumberMasker.Mask(proposal.CreatorCommercialRegisterNumber);
        }

        return timeZoneConverter.ConvertUtcToLocal(result);
    }
}
