using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Talaby.Domain.Entities.Projects;
using Talaby.Domain.Exceptions;
using Talaby.Domain.Repositories.Projects;

namespace Talaby.Application.Projects.QuestionReplies.Commands.UpdateQuestionReply;

public class UpdateQuestionReplyCommandHandler(ILogger<UpdateQuestionReplyCommandHandler> logger,
    IQuestionReplyRepository questionReplyRepository, IMapper mapper) : IRequestHandler<UpdateQuestionReplyCommand>
{
    public async Task Handle(UpdateQuestionReplyCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating QuestionReply with id: {QuestionReplyId} with {@UpdatedQuestionReply}", request.Id, request);
        var questionReply = await questionReplyRepository.GetByIdAsync(request.Id);
        if (questionReply is null)
            throw new NotFoundException(nameof(QuestionReply), request.Id.ToString());

        var existingQuestionReply = await questionReplyRepository.AnyAsync( p => p.Id == request.Id  , cancellationToken);

        if (!existingQuestionReply)
        {
            throw new NotFoundException(nameof(QuestionReply), request.Id.ToString());
        }

        mapper.Map(request, questionReply);

        await questionReplyRepository.SaveChanges();
    }
}
