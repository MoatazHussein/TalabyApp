using MediatR;

namespace Talaby.Application.Features.Projects.ProjectRequests.Commands.CreateProjectRequest;

public record CreateProjectRequestCommand(
string Title,
string Description,
string ImageUrl,
decimal MinBudget,
decimal MaxBudget,
int StoreCategoryId
) : IRequest<Guid>;
