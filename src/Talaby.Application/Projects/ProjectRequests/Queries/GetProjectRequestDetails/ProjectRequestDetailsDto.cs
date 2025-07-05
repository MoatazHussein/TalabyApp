namespace Talaby.Application.Projects.ProjectRequests.Queries.GetProjectRequestDetails;

public class ProjectRequestDetailsDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public string CreatorEmail { get; set; } = default!;

    public List<ProjectProposalDto> Proposals { get; set; } = new();
}
