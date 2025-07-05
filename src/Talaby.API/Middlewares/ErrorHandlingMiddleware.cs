using Microsoft.AspNetCore.Http;
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
        catch (ForbidException)
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("Access forbidden");
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
        catch (BusinessRuleException ex) 
        {
            await context.Response.WriteAsync(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);

            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Something went wrong");
        }
    }
}