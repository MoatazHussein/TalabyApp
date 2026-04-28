using MediatR;
using Microsoft.Extensions.Logging;
using Talaby.Application.Features.Users.Services;
using Talaby.Domain.Entities.Projects;
using Talaby.Domain.Exceptions;
using Talaby.Domain.Repositories.Projects;

namespace Talaby.Application.Features.Projects.QuestionReplies.Commands.DeleteQuestionReply
{
    internal class DeleteQuestionReplyCommandHandler(ILogger<DeleteQuestionReplyCommandHandler> logger,IUserContext userContext,
    IQuestionReplyRepository questionReplyRepository) : IRequestHandler<DeleteQuestionReplyCommand>
    {
        public async Task Handle(DeleteQuestionReplyCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Deleting QuestionReply with id: {QuestionReplyId}", request.Id);
            var questionReply = await questionReplyRepository.GetByIdAsync(request.Id);
            if (questionReply is null)
                throw new NotFoundException(nameof(QuestionReply), request.Id.ToString());


            var currentUser = userContext.GetCurrentUser()
                    ?? throw new UnAuthorizedAccessException("User not authenticated.");

            if (questionReply.CreatorId != currentUser.Id)
                throw new BusinessRuleException("You are not allowed to delete this Reply.", 403);

            await questionReplyRepository.Delete(questionReply);

        }
    }
}
