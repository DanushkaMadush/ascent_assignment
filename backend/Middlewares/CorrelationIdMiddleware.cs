using Microsoft.Extensions.Primitives;

namespace backend.Middlewares
{
    public class CorrelationIdMiddleware
    {
        private const string HeaderName = "X-Correlation-ID";

        private readonly RequestDelegate _next;
        private readonly ILogger<CorrelationIdMiddleware> _logger;

        public CorrelationIdMiddleware(
            RequestDelegate next,
            ILogger<CorrelationIdMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = GetCorrelationId(context);

            context.Response.Headers[HeaderName] = correlationId;

            using (_logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = correlationId
            }))
            {
                await _next(context);
            }
        }

        private static string GetCorrelationId(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue(
                    HeaderName,
                    out StringValues value) &&
                !StringValues.IsNullOrEmpty(value))
            {
                var correlationId = value.ToString().Trim();

                if (correlationId.Length <= 100)
                {
                    return correlationId;
                }
            }

            return Guid.NewGuid().ToString("N");
        }
    }
}
