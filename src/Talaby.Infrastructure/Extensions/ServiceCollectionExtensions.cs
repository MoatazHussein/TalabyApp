using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Talaby.Application.Common.Interfaces;
using Talaby.Domain.Entities;
using Talaby.Domain.Repositories;
using Talaby.Domain.Repositories.Projects;
using Talaby.Infrastructure.Email;
using Talaby.Infrastructure.Identity;
using Talaby.Infrastructure.Persistence;
using Talaby.Infrastructure.Repositories;
using Talaby.Infrastructure.Repositories.Projects;
using Talaby.Infrastructure.Seeders;

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


    }

}
