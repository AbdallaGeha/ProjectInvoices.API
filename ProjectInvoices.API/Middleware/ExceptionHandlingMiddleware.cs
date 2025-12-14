using ProjectInvoices.API.Exceptions;
using System.Net;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace ProjectInvoices.API.Middleware
{
    /// <summary>
    /// Exception Handling Middleware class, When we encounter an unhandled exception
    /// in the pipeline we return 500 error response along with string message
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            string message = exception.Message;
            object response;

            switch (exception)
            {
                case NotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    response = new
                    {
                        error = message
                    };
                    break;

                case DuplicateException:
                    statusCode = HttpStatusCode.Conflict;
                    response = new
                    {
                        error = message
                    };
                    break;

                case BusinessException:
                    statusCode = HttpStatusCode.BadRequest;
                    response = new
                    {
                        error = message
                    };
                    break;

                case UnauthorizedAccessException:
                    statusCode = HttpStatusCode.Unauthorized;
                    response = new
                    {
                        error = message
                    };
                    break;

                case ValidationException:
                    statusCode = HttpStatusCode.BadRequest;
                    response = new
                    {
                        error = message
                    };
                    break;

                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    response = new { error = "An unexpected error occurred" };
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
