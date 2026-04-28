using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Talaby.Application.Features.Users.Services;
using Talaby.Domain.Entities.Projects;
using Talaby.Domain.Exceptions;
using Talaby.Domain.Repositories.Projects;

namespace Talaby.Application.Features.Projects.ProposalReplies.Commands.UpdateProposalReply;

public class UpdateProposalReplyCommandHandler(ILogger<UpdateProposalReplyCommandHandler> logger, IUserContext userContext,
    IProposalReplyRepository proposalReplyRepository, IMapper mapper) : IRequestHandler<UpdateProposalReplyCommand>
{
    public async Task Handle(UpdateProposalReplyCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating ProposalReply with id: {ProposalReplyId} with {@UpdatedProposalReply}", request.Id, request);
        var proposalReply = await proposalReplyRepository.GetByIdAsync(request.Id);
        if (proposalReply is null)
            throw new NotFoundException(nameof(ProposalReply), request.Id.ToString());

        var currentUser = userContext.GetCurrentUser()
                         ?? throw new UnAuthorizedAccessException("User not authenticated.");

        if (proposalReply.CreatorId != currentUser.Id)
            throw new BusinessRuleException("You are not allowed to Update this Reply.", 403);

        mapper.Map(request, proposalReply);

        await proposalReplyRepository.SaveChanges();
    }
}
