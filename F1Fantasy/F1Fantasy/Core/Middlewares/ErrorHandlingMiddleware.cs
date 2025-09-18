using F1Fantasy.Exceptions;

namespace F1Fantasy.Core.Middlewares;

public class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (NotFoundException ex)
        {
            logger.LogError(ex, "Not found exception");
            context.Response.StatusCode = 400;
            context.Response.ContentType = "application/json";
            var errorResponse = new
            {
                error = "An not found error occurred.",
                message = ex.Message,
            };
            await context.Response.WriteAsJsonAsync(errorResponse);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception");
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            var errorResponse = new
            {
                error = "An error occurred.",
                message = ex.Message,
            };
            await context.Response.WriteAsJsonAsync(errorResponse);
        }
    }
}