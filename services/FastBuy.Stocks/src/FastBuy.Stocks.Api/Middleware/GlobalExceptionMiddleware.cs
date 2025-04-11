namespace FastBuy.Stocks.Api.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next,ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;

        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

            } catch (Exception ex)
            {

                await HandleExeptionAsync(context,ex);
            }
        }

        private Task HandleExeptionAsync(HttpContext context,Exception ex)
        {
            _logger.LogError($"Ocurrió un error inesperado - {ex.Message} - {DateTimeOffset.UtcNow.ToString("dd/MM/yyyy HH:mm:ss")}");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = ex switch
            {
                KeyNotFoundException => StatusCodes.Status404NotFound,
                ArgumentException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError //Excepciones generales
            };

            var response = new
            {
                message = "Ocurrio un error insesperado.",
                statusCode = context.Response.StatusCode
            };

            return context.Response.WriteAsJsonAsync(response);
        }
    }
}
