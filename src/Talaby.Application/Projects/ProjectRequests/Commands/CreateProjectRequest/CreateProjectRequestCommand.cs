using MediatR;

namespace Talaby.Application.Projects.ProjectRequests.Commands.CreateProjectRequest;

public record CreateProjectRequestCommand(
string Title,
string Description,
decimal MinBudget,
decimal MaxBudget,
int StoreCategoryId
) : IRequest<Guid>;
