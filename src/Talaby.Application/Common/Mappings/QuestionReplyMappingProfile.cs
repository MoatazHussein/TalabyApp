using AutoMapper;
using Talaby.Application.Projects.QuestionReplies.Commands.UpdateQuestionReply;
using Talaby.Domain.Entities.Projects;

namespace Talaby.Application.Common.Mappings;

public class QuestionReplyMappingProfile : Profile
{
    public QuestionReplyMappingProfile()
    {
        CreateMap<UpdateQuestionReplyCommand, QuestionReply>();
    }
}
