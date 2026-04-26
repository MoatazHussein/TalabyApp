using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Talaby.API.Middlewares;
using Talaby.Application.Common;
using Serilog;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using System.Text.Json.Serialization;

namespace Talaby.API.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static void AddPresentation(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers()
                        .AddJsonOptions(opts => opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        builder.Services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = ctx =>
            {
                var errors = ctx.ModelState
                    .Where(e => e.Value?.Errors.Count > 0)
                    .SelectMany(e => e.Value!.Errors.Select(x => new FieldError(e.Key, x.ErrorMessage)))
                    .ToList();

                var response = ApiResponse.Fail("Validation failed", errorCode: "VALIDATION_ERROR", errors: errors);
                return new ObjectResult(response) { StatusCode = 400 };
            };
        });
        builder.Services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "bearerAuth"}
                    },
                    []
                }

            });
        });


        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            )
        };
    });

        builder.Services.AddAuthorization();

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddScoped<ErrorHandlingMiddleware>();
        //builder.Services.AddScoped<RequestTimeLoggingMiddleware>();

        builder.Host.UseSerilog((context, configuration) =>
            configuration.ReadFrom.Configuration(context.Configuration)
        );
    }
}