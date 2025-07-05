using MediatR;
using Talaby.Application.Users;
using Talaby.Domain.Entities.Projects;
using Talaby.Domain.Repositories.Projects;

namespace Talaby.Application.Projects.ProjectRequests.Commands.CreateProjectRequest;

public class CreateProjectRequestCommandHandler(IProjectRequestRepository repository, IUserContext userContext) : IRequestHandler<CreateProjectRequestCommand, Guid>
{
    private readonly IProjectRequestRepository _repository = repository;
    private readonly IUserContext _userContext = userContext;

    public async Task<Guid> Handle(CreateProjectRequestCommand request, CancellationToken cancellationToken)
    {

        var entity = new ProjectRequest
        {
            Title = request.Title,
            Description = request.Description,
            MinBudget = request.MinBudget,
            MaxBudget = request.MaxBudget,
            StoreCategoryId = request.StoreCategoryId,
            CreatorId = _userContext.GetCurrentUser().Id,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.Create(entity);
        return entity.Id;
    }
}
