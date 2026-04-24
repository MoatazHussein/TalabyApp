using MediatR;

namespace Talaby.Application.Features.Projects.ProjectRequests.Commands.MarkProjectRequestAsDone;

public record MarkProjectRequestAsDoneCommand(Guid ProjectRequestId) : IRequest;
