namespace ProjectInvoices.API.Middleware
{
    /// <summary>
    /// Exception Handling Middleware class, When we encounter an unhandled exception
    /// in the pipeline we return 500 error response along with string message
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                httpContext.Response.StatusCode = 500;
                await httpContext.Response.WriteAsJsonAsync(ex.Message);
            }
        }
    }
}
