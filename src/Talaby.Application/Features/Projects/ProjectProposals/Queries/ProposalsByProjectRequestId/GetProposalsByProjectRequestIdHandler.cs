using System.Threading.Tasks;
using MediatR;
using Talaby.Application.Common;
using Talaby.Application.Common.Interfaces;

namespace Talaby.Application.Features.Projects.ProjectProposals.Queries.ProposalsByProjectRequestId;

public class GetProposalsByProjectRequestIdHandler(IProjectProposalReadRepository repository, ICommercialRegisterNumberMasker mask,
     ITimeZoneConverter timeZoneConverter)
        : IRequestHandler<GetProposalsByProjectRequestIdQuery, PagedResult<ProjectProposalListItemDto>>
{
    public async Task<PagedResult<ProjectProposalListItemDto>> Handle(
        GetProposalsByProjectRequestIdQuery request,
        CancellationToken cancellationToken)
    {
       
        var result = await repository.GetPagedProposalsAsync(
            request.ProjectRequestId,
            request.PageNumber,
            request.PageSize,
            cancellationToken);


        foreach (var reply in result.Items)
        {
            reply.CreatorCommercialRegisterNumber =
                mask.Mask(reply.CreatorCommercialRegisterNumber);
        }

        return timeZoneConverter.ConvertUtcToLocal(result);
    }
}
