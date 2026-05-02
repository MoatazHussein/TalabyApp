using MediatR;
using Talaby.Domain.Enums;


namespace Talaby.Application.Features.Projects.ProjectProposals.Commands.UpdateProjectProposalStatus;

public record UpdateProjectProposalStatusCommand(
    Guid Id,
    ProjectProposalStatus NewStatus,
    string? CancellationReason)
    : IRequest;

