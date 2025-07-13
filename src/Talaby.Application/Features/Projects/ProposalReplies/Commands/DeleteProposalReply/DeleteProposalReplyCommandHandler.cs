using MediatR;
using Microsoft.Extensions.Logging;
using Talaby.Domain.Entities.Projects;
using Talaby.Domain.Exceptions;
using Talaby.Domain.Repositories.Projects;

namespace Talaby.Application.Features.Projects.ProposalReplies.Commands.DeleteProposalReply
{
    internal class DeleteProposalReplyCommandHandler(ILogger<DeleteProposalReplyCommandHandler> logger,
    IProposalReplyRepository proposalReplyRepository) : IRequestHandler<DeleteProposalReplyCommand>
    {
        public async Task Handle(DeleteProposalReplyCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Deleting ProposalReply with id: {ProposalReplyId}", request.Id);
            var proposalReply = await proposalReplyRepository.GetByIdAsync(request.Id);
            if (proposalReply is null)
                throw new NotFoundException(nameof(ProposalReply), request.Id.ToString());

            await proposalReplyRepository.Delete(proposalReply);

        }
    }
}
