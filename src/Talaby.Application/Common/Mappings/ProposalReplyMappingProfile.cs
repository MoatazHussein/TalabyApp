using AutoMapper;
using Talaby.Application.Features.Projects.ProposalReplies.Commands.UpdateProposalReply;
using Talaby.Domain.Entities.Projects;

namespace Talaby.Application.Common.Mappings;

public class ProposalReplyMappingProfile : Profile
{
    public ProposalReplyMappingProfile()
    {
        CreateMap<UpdateProposalReplyCommand, ProposalReply>();
    }
}
