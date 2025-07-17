using MediatR;
using Talaby.Domain.Enums;


namespace Talaby.Application.Features.Projects.ProjectRequests.Commands.UpdateProjectRequestStatus;

public record UpdateProjectRequestStatusCommand(Guid Id, ProjectRequestStatus NewStatus)
    : IRequest;
