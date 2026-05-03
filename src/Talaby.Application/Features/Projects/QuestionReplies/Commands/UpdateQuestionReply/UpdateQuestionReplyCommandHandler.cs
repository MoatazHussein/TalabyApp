using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Talaby.Application.Common.Interfaces;
using Talaby.Application.Features.Users.Services;
using Talaby.Domain.Entities.Projects;
using Talaby.Domain.Exceptions;
using Talaby.Domain.Repositories.Projects;

namespace Talaby.Application.Features.Projects.QuestionReplies.Commands.UpdateQuestionReply;

public class UpdateQuestionReplyCommandHandler(ILogger<UpdateQuestionReplyCommandHandler> logger,IUserContext userContext,
    IQuestionReplyRepository questionReplyRepository, IMapper mapper, IUnitOfWork unitOfWork) : IRequestHandler<UpdateQuestionReplyCommand>
{
    public async Task Handle(UpdateQuestionReplyCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating QuestionReply with id: {QuestionReplyId} with {@UpdatedQuestionReply}", request.Id, request);
        var questionReply = await questionReplyRepository.GetByIdAsync(request.Id);
        if (questionReply is null)
            throw new NotFoundException(nameof(QuestionReply), request.Id.ToString());


        var currentUser = userContext.GetCurrentUser()
                ?? throw new UnAuthorizedAccessException("User not authenticated.");

        if (questionReply.CreatorId != currentUser.Id)
            throw new BusinessRuleException("You are not allowed to update this Reply.", 403);


        mapper.Map(request, questionReply);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
