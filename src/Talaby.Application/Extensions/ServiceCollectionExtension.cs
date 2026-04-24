using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Talaby.Application.Common;
using Talaby.Application.Features.Users;

namespace Talaby.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {

        var applicationAssembly = typeof(ServiceCollectionExtensions).Assembly;

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(applicationAssembly));

        services.AddAutoMapper(applicationAssembly);

        services.AddValidatorsFromAssembly(applicationAssembly)
           .AddFluentValidationAutoValidation();

        services.AddScoped<IUserContext, UserContext>();
        services.AddHttpContextAccessor();

        services.AddOptions<TapCheckoutOptions>()
            .Bind(configuration.GetSection(TapCheckoutOptions.SectionName))
            .Validate(opts => opts.CommissionPercentage > 0, "Tap:CommissionPercentage must be greater than zero.")
            .ValidateOnStart();

    }
}
