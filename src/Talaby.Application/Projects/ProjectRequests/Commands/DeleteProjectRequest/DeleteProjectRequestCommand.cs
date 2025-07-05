using MediatR;

namespace Talaby.Application.Projects.ProjectRequests.Commands.DeleteProjectRequest;

public class DeleteProjectRequestCommand(Guid id) : IRequest
{
    public Guid Id { get; } = id;
}
