using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Talaby.Application.Common.Interfaces;
using Talaby.Application.Features.Projects.ProjectProposals.Queries.ProposalsByProjectRequestId;
using Talaby.Application.Features.Projects.ProjectQuestions.Queries.QuestionsByProjectRequestId;
using Talaby.Application.Features.Projects.ProjectRequests.Queries.GetProjectRequestDetails;
using Talaby.Application.Features.Projects.ProposalReplies.Queries.RepliesByProposalId;
using Talaby.Application.Features.Projects.QuestionReplies.Queries.RepliesByQuestionId;
using Talaby.Domain.Entities;
using Talaby.Domain.Repositories;
using Talaby.Domain.Repositories.Projects;
using Talaby.Infrastructure.Persistence;
using Talaby.Infrastructure.Repositories;
using Talaby.Infrastructure.Repositories.Projects;
using Talaby.Infrastructure.Seeders;
using Talaby.Infrastructure.Services.DataVisibilityPolicy;
using Talaby.Infrastructure.Services.Email;
using Talaby.Infrastructure.Services.Identity;
using Talaby.Infrastructure.Services.TimeConversion;
using Talaby.Infrastructure.Services.UnitOfWork;

namespace Talaby.Infrastructure.Extensions;
public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("TalabyDb");

        services.AddDbContext<TalabyDbContext>(options =>
            options.UseSqlServer(connectionString)
         .EnableSensitiveDataLogging());

        services.AddScoped<IMailService, MailService>();

        services.AddScoped<IJwtService, JwtService>();

        services.AddIdentityApiEndpoints<AppUser>()
        .AddRoles<AppRole>()
        .AddEntityFrameworkStores<TalabyDbContext>();

        services.AddScoped<ITalabySeeder, TalabySeeder>();
        services.AddScoped<IStoreCategoryRepository, StoreCategoryRepository>();
        services.AddScoped<IProjectRequestRepository, ProjectRequestRepository>();
        services.AddScoped<IProjectProposalRepository, ProjectProposalRepository>();
        services.AddScoped<IProposalReplyRepository, ProposalReplyRepository>();
        services.AddScoped<IProjectRequestDetailsRepository, ProjectRequestDetailsRepository>();
        services.AddScoped<IProjectProposalReadRepository, ProjectProposalReadRepository>();
        services.AddScoped<IProposalReplyReadRepository, ProposalReplyReadRepository>();


        services.AddScoped<IProjectQuestionRepository, ProjectQuestionRepository>();
        services.AddScoped<IQuestionReplyRepository, QuestionReplyRepository>();
        services.AddScoped<IProjectQuestionReadRepository, ProjectQuestionReadRepository>();
        services.AddScoped<IQuestionReplyReadRepository, QuestionReplyReadRepository>();


        services.AddScoped<ICommercialRegisterNumberMasker, CommercialRegisterNumberMasker>();
        services.AddScoped<ITimeZoneConverter, TimeZoneConverter>();
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();

    }

}
