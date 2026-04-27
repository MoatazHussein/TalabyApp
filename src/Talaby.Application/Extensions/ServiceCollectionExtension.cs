using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Talaby.Application.Common;
using Talaby.Application.Common.Behaviors;
using Talaby.Application.Features.Users;

namespace Talaby.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {

        var applicationAssembly = typeof(ServiceCollectionExtensions).Assembly;

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(applicationAssembly));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddAutoMapper(applicationAssembly);

        services.AddValidatorsFromAssembly(applicationAssembly);

        services.AddScoped<IUserContext, UserContext>();
        services.AddHttpContextAccessor();

        services.AddOptions<TapCheckoutOptions>()
            .Bind(configuration.GetSection(TapCheckoutOptions.SectionName))
            .Validate(opts => opts.CommissionPercentage > 0, "Tap:CommissionPercentage must be greater than zero.")
            .ValidateOnStart();

    }
}
