namespace Talaby.Domain.Entities.Projects;

public class ProposalReply
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ProjectProposalId { get; set; }
    public ProjectProposal? ProjectProposal { get; set; }

    public string Content { get; set; } = default!;

    public Guid CreatorId { get; set; }
    public AppUser? Creator { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
