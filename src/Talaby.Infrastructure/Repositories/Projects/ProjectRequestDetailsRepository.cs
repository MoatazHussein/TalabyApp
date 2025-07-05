using Microsoft.EntityFrameworkCore;
using Talaby.Application.Projects.Dtos;
using Talaby.Application.Projects.ProjectRequests.Queries.GetProjectRequestDetails;
using Talaby.Infrastructure.Persistence;

namespace Talaby.Infrastructure.Repositories.Projects;

public class ProjectRequestDetailsRepository : IProjectRequestDetailsRepository
{
    private readonly TalabyDbContext _context;

    public ProjectRequestDetailsRepository(TalabyDbContext context)
    {
        _context = context;
    }

    public async Task<ProjectRequestDetailsDto?> GetDetailsAsync(Guid requestId, CancellationToken cancellationToken)
    {
        var entity = await _context.ProjectRequests
            .Include(r => r.Creator)
            .Include(r => r.Proposals)
                .ThenInclude(p => p.Creator)
            .Include(r => r.Proposals)
                .ThenInclude(p => p.Replies)
                    .ThenInclude(r => r.Creator)
            .FirstOrDefaultAsync(r => r.Id == requestId, cancellationToken);

        if (entity is null)
            return null;

        return new ProjectRequestDetailsDto
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            CreatedAt = entity.CreatedAt,
            CreatorEmail = entity.Creator?.Email ?? "N/A",
            Proposals = entity.Proposals.Select(p => new ProjectProposalDto
            {
                Id = p.Id,
                Content = p.Content,
                ProposedAmount = p.ProposedAmount,
                CreatedAt = p.CreatedAt,
                CreatorEmail = p.Creator?.Email ?? "N/A",
                Replies = p.Replies.Select(r => new ProposalReplyDto
                {
                    Id = r.Id,
                    Content = r.Content,
                    CreatedAt = r.CreatedAt,
                    CreatorEmail = r.Creator?.Email ?? "N/A"
                }).ToList()
            }).ToList()
        };
    }
}
