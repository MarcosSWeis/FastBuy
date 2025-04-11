namespace FastBuy.Products.Api.Middleware
{
    internal class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        public GlobalExceptionMiddleware(RequestDelegate next,ILogger<GlobalExceptionMiddleware> loger)
        {
            _next = next;
            _logger = loger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

            } catch (Exception ex)
            {
                await HandleExceptionAsync(context,ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context,Exception exception)
        {
            _logger.LogError($"Ocurrió un error inesperado - {exception.Message} - {DateTimeOffset.UtcNow.ToString("dd/MM/yyyy HH:mm:ss")}");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = exception switch
            {
                KeyNotFoundException => StatusCodes.Status404NotFound,
                ArgumentException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError //Excepciones generales
            };

            var response = new
            {
                message = "Ocurrió un error inesperado.",
                statusCode = context.Response.StatusCode,
            };

            return context.Response.WriteAsJsonAsync(response);
        }

    }
}
