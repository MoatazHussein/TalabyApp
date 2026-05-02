using Talaby.Domain.Enums;

namespace Talaby.Application.Features.Projects.ProjectRequests.Queries.Dtos;

public class ProjectRequestDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string? ImageUrl { get; set; }
    public decimal MinBudget { get; set; }
    public decimal MaxBudget { get; set; }
    public int StoreCategoryId { get; set; }
    public Guid CreatorId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ProjectRequestStatus Status { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime? CancelledAt { get; set; }
    public Guid? CancelledByUserId { get; set; }
}
