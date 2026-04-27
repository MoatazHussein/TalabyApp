using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using Talaby.Application.Common.Interfaces;
using Talaby.Application.Features.Payments.Contracts;
using Talaby.Application.Features.Projects.ProjectProposals.Queries.ProposalsByProjectRequestId;
using Talaby.Application.Features.Projects.ProjectQuestions.Queries.QuestionsByProjectRequestId;
using Talaby.Application.Features.Projects.ProposalReplies.Queries.RepliesByProposalId;
using Talaby.Application.Features.Projects.QuestionReplies.Queries.RepliesByQuestionId;
using Talaby.Domain.Entities;
using Talaby.Domain.Repositories;
using Talaby.Domain.Repositories.Payments;
using Talaby.Domain.Repositories.Projects;
using Talaby.Infrastructure.Payments;
using Talaby.Infrastructure.Payments.Configuration;
using Talaby.Infrastructure.Persistence;
using Talaby.Infrastructure.Repositories;
using Talaby.Infrastructure.Repositories.Payments;
using Talaby.Infrastructure.Repositories.Projects;
using Talaby.Infrastructure.Seeders;
using Talaby.Infrastructure.Services.DataVisibilityPolicy;
using Talaby.Infrastructure.Services.Email;
using Talaby.Infrastructure.Services.Identity;
using Talaby.Infrastructure.Services.TimeConversion;
using Talaby.Infrastructure.Services.UnitOfWork;
using Talaby.Application.Features.Dashboard.Queries.Admin;
using Talaby.Application.Features.Dashboard.Queries.Client;
using Talaby.Application.Features.Dashboard.Queries.Store;

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
        services.AddScoped<IProjectProposalReadRepository, ProjectProposalReadRepository>();
        services.AddScoped<IProposalReplyReadRepository, ProposalReplyReadRepository>();


        services.AddScoped<IProjectQuestionRepository, ProjectQuestionRepository>();
        services.AddScoped<IQuestionReplyRepository, QuestionReplyRepository>();
        services.AddScoped<IProjectQuestionReadRepository, ProjectQuestionReadRepository>();
        services.AddScoped<IQuestionReplyReadRepository, QuestionReplyReadRepository>();

        services.AddScoped<IProjectCommissionPaymentRepository, ProjectCommissionPaymentRepository>();
        services.AddScoped<IAdminDashboardReadRepository, DashboardReadRepository>();
        services.AddScoped<IClientDashboardReadRepository, DashboardReadRepository>();
        services.AddScoped<IStoreDashboardReadRepository, DashboardReadRepository>();

        // Tap HTTP client — auth header is set once at registration time from config.
        // TapPaymentService uses IHttpClientFactory to get a pre-configured client.
        services.AddHttpClient("TapClient", (sp, client) =>
        {
            var tapOpts = sp.GetRequiredService<IOptions<TapOptions>>().Value;
            client.BaseAddress = new Uri(tapOpts.BaseUrl);
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", tapOpts.SecretKey);
        });

        services.AddScoped<ITapPaymentService, TapPaymentService>();
        services.AddScoped<ITapWebhookValidator, TapWebhookValidator>();

        services.AddScoped<ICommercialRegisterNumberMasker, CommercialRegisterNumberMasker>();

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<ITimeZoneConverter, TimeZoneConverter>();
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();


        services.AddOptions<TapOptions>()
            .Bind(configuration.GetSection(TapOptions.SectionName))
            .ValidateDataAnnotations()
            .Validate(options => !string.IsNullOrWhiteSpace(options.BaseUrl), "Tap:BaseUrl is required.")
            .ValidateOnStart();

    }

}
