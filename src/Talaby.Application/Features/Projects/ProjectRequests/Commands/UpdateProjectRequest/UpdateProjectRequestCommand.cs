using MediatR;
namespace Talaby.Application.Features.Projects.ProjectRequests.Commands.UpdateProjectRequest;

public class UpdateProjectRequestCommand : IRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string? ImageUrl { get; set; }
    public decimal MinBudget { get; set; }
    public decimal MaxBudget { get; set; }
    public int StoreCategoryId { get; set; }

}