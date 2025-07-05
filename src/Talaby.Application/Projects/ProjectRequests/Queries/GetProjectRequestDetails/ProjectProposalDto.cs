using Talaby.Application.Projects.Dtos;

namespace Talaby.Application.Projects.ProjectRequests.Queries.GetProjectRequestDetails;

public class ProjectProposalDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = default!;
    public decimal ProposedAmount { get; set; }
    public string CreatorEmail { get; set; } = default!;
    public DateTime CreatedAt { get; set; }

    public List<ProposalReplyDto> Replies { get; set; } = new();
}
