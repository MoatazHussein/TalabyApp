using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Talaby.Domain.Entities.Projects;
using Talaby.Domain.Exceptions;
using Talaby.Domain.Repositories.Projects;

namespace Talaby.Application.Projects.ProposalReplies.Commands.UpdateProposalReply;

public class UpdateProposalReplyCommandHandler(ILogger<UpdateProposalReplyCommandHandler> logger,
    IProposalReplyRepository proposalReplyRepository, IMapper mapper) : IRequestHandler<UpdateProposalReplyCommand>
{
    public async Task Handle(UpdateProposalReplyCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating ProposalReply with id: {ProposalReplyId} with {@UpdatedProposalReply}", request.Id, request);
        var proposalReply = await proposalReplyRepository.GetByIdAsync(request.Id);
        if (proposalReply is null)
            throw new NotFoundException(nameof(ProposalReply), request.Id.ToString());

        var existingProposalReply = await proposalReplyRepository.AnyAsync( p => p.Id == request.Id  , cancellationToken);

        if (!existingProposalReply)
        {
            throw new NotFoundException(nameof(ProposalReply), request.Id.ToString());
        }

        mapper.Map(request, proposalReply);

        await proposalReplyRepository.SaveChanges();
    }
}
