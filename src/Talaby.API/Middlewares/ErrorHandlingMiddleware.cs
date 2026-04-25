using Microsoft.EntityFrameworkCore;
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
            context.Response.StatusCode = 404;
            await context.Response.WriteAsync(notFound.Message);

            logger.LogWarning(notFound.Message);
        }
        catch (UnAuthorizedAccessException unAuthorizedAccess)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync(unAuthorizedAccess.Message);
        }
        catch (AlreadyExistsException alreadyExists)
        {
            context.Response.StatusCode = 409;
            await context.Response.WriteAsync(alreadyExists.Message);
        }
        catch (AppException ex)
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync(ex.Message);
        }
        catch (DbUpdateException)
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Database update failed");
        }
        catch (PaymentGatewayException ex)
        {
            logger.LogWarning(ex,
                "Payment gateway error. Path={Path}",
                context.Request.Path);

            context.Response.StatusCode = 502;
            await context.Response.WriteAsync("Payment gateway error. Please try again later.");
        }
        catch (BusinessRuleException ex)
        {
            if (ex.StatusCode >= 500)
            {
                logger.LogWarning(ex,
                    "Business rule exception with server-side status. StatusCode={StatusCode}, Path={Path}",
                    ex.StatusCode, context.Request.Path);
            }

            context.Response.StatusCode = ex.StatusCode;
            await context.Response.WriteAsync(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Unhandled exception. Path={Path}, Method={Method}",
                context.Request.Path, context.Request.Method);

            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Something went wrong");
        }
    }
}