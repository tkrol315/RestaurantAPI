using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace RestaurantAPI.Middleware
{
    public class RequestStopwatchMiddleware : IMiddleware
    {
        private readonly ILogger _logger;
        private Stopwatch _stopwatch;

        public RequestStopwatchMiddleware(ILogger<RequestStopwatchMiddleware> logger)
        {
            _logger = logger;
            _stopwatch = new Stopwatch();
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            _stopwatch.Start();
            await next.Invoke(context);
            _stopwatch.Stop();
            var measuredTime = _stopwatch.ElapsedMilliseconds;
            if (measuredTime > 4000)
            {
                var message = $"Request {context.Request.Method} at {context.Request.Path} took {measuredTime} ms";
                _logger.LogInformation(message);
            }
        }
    }
}