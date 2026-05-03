using MediatR;
using Microsoft.Extensions.Logging;
using Talaby.Application.Common.Interfaces;
using Talaby.Application.Features.Users.Services;
using Talaby.Domain.Entities.Projects;
using Talaby.Domain.Exceptions;
using Talaby.Domain.Repositories.Projects;

namespace Talaby.Application.Features.Projects.ProposalReplies.Commands.DeleteProposalReply
{
    internal class DeleteProposalReplyCommandHandler(ILogger<DeleteProposalReplyCommandHandler> logger,IUserContext userContext,
    IProposalReplyRepository proposalReplyRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteProposalReplyCommand>
    {
        public async Task Handle(DeleteProposalReplyCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Deleting ProposalReply with id: {ProposalReplyId}", request.Id);
            var proposalReply = await proposalReplyRepository.GetByIdAsync(request.Id);
            if (proposalReply is null)
                throw new NotFoundException(nameof(ProposalReply), request.Id.ToString());

            var currentUser = userContext.GetCurrentUser()
                       ?? throw new UnAuthorizedAccessException("User not authenticated.");

            if (proposalReply.CreatorId != currentUser.Id)
                throw new BusinessRuleException("You are not allowed to delete this Reply.", 403);

            await proposalReplyRepository.Delete(proposalReply);
            await unitOfWork.SaveChangesAsync(cancellationToken);

        }
    }
}
