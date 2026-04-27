using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Talaby.Application.Common;
using Talaby.Domain.Exceptions;

namespace Talaby.API.Middlewares;

public class ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next.Invoke(context);
        }
        catch (NotFoundException notFound)
        {
            logger.LogWarning(notFound.Message);
            await WriteErrorAsync(context, 404, notFound.Message);
        }
        catch (UnAuthorizedAccessException unAuthorizedAccess)
        {
            await WriteErrorAsync(context, 401, unAuthorizedAccess.Message);
        }
        catch (AlreadyExistsException alreadyExists)
        {
            await WriteErrorAsync(context, 409, alreadyExists.Message);
        }
        catch (AppException ex)
        {
            await WriteErrorAsync(context, 500, ex.Message);
        }
        catch (DbUpdateException)
        {
            await WriteErrorAsync(context, 500, "Database update failed");
        }
        catch (PaymentGatewayException ex)
        {
            logger.LogWarning(ex, "Payment gateway error. Path={Path}", context.Request.Path);
            await WriteErrorAsync(context, 502, "Payment gateway error. Please try again later.");
        }
        catch (BusinessRuleException ex)
        {
            if (ex.StatusCode >= 500)
            {
                logger.LogWarning(ex,
                    "Business rule exception with server-side status. StatusCode={StatusCode}, Path={Path}",
                    ex.StatusCode, context.Request.Path);
            }

            await WriteErrorAsync(context, ex.StatusCode, ex.Message, ex.ErrorCode);
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors
                .Select(error => new FieldError(error.PropertyName, error.ErrorMessage))
                .ToList();

            await WriteErrorAsync(context, 400, "Validation failed", "VALIDATION_ERROR", errors);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Unhandled exception. Path={Path}, Method={Method}",
                context.Request.Path, context.Request.Method);

            await WriteErrorAsync(context, 500, "Something went wrong");
        }
    }

    private static Task WriteErrorAsync(
        HttpContext context,
        int statusCode,
        string message,
        string? errorCode = null,
        IReadOnlyList<FieldError>? errors = null)
    {
        context.Response.StatusCode = statusCode;
        return context.Response.WriteAsJsonAsync(ApiResponse.Fail(message, errorCode, errors));
    }
}
