using MediatR;
using Talaby.Application.Features.Projects.ProjectRequests.Queries.Dtos;

namespace Talaby.Application.Features.Projects.ProjectRequests.Queries.GetProjectRequestById;

public class GetProjectRequestByIdQuery(Guid id) : IRequest<ProjectRequestDto>
{
    public Guid Id { get;} = id;
}
