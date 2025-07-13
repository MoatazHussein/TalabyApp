using AutoMapper;
using Talaby.Application.Features.Projects.QuestionReplies.Commands.UpdateQuestionReply;
using Talaby.Domain.Entities.Projects;

namespace Talaby.Application.Common.Mappings;

public class QuestionReplyMappingProfile : Profile
{
    public QuestionReplyMappingProfile()
    {
        CreateMap<UpdateQuestionReplyCommand, QuestionReply>();
    }
}
