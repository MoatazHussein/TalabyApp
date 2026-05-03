using MediatR;
using Talaby.Application.Common.Interfaces;
using Talaby.Application.Features.Users.Services;
using Talaby.Domain.Entities.Projects;
using Talaby.Domain.Exceptions;
using Talaby.Domain.Repositories.Projects;

namespace Talaby.Application.Features.Projects.ProjectProposals.Commands.CreateProjectProposal;

public class CreateProjectProposalCommandHandler(
    IProjectProposalRepository repository,
    IUserContext userContext,
    IUserConfirmationGuard userConfirmationGuard,
    IUserActionGuard userActionGuard,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateProjectProposalCommand, Guid>
{
    public async Task<Guid> Handle(CreateProjectProposalCommand request, CancellationToken cancellationToken)
    {
        await userConfirmationGuard.EnsureCurrentUserEmailConfirmedAsync(cancellationToken);

        var currentUser = userContext.GetCurrentUser();

        await userActionGuard.EnsureCanCreateProposalAsync(currentUser.Id, cancellationToken);

        var hasExistingProposal = await repository.AnyAsync(
            proposal => proposal.ProjectRequestId == request.ProjectRequestId
                        && proposal.CreatorId == currentUser.Id,
            cancellationToken);

        if (hasExistingProposal)
        {
            throw new BusinessRuleException(
                "You already created a proposal for this project request.",
                409,
                "PROJECT_PROPOSAL_ALREADY_EXISTS");
        }

        var proposal = new ProjectProposal
        {
            Id = Guid.NewGuid(),
            ProjectRequestId = request.ProjectRequestId,
            Content = request.Content,
            ProposedAmount = request.ProposedAmount,
            CreatorId = currentUser.Id,
            CreatedAt = DateTime.UtcNow
        };

        await repository.Create(proposal);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return proposal.Id;
    }
}
