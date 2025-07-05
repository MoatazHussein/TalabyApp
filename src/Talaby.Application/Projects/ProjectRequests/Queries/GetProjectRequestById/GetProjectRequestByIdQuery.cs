using MediatR;
using Talaby.Application.Projects.ProjectRequests.Dtos;

namespace Talaby.Application.Projects.ProjectRequests.Queries.GetProjectRequestById;

public class GetProjectRequestByIdQuery(Guid id) : IRequest<ProjectRequestDto>
{
    public Guid Id { get;} = id;
}
