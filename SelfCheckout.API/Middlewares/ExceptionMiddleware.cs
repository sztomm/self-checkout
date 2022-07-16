using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace SelfCheckout.API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate Next;
        private readonly ILogger<ExceptionMiddleware> Logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            Next = next;
            Logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await Next(httpContext).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Something went wrong: {ex}");
                await HandleExceptionAsync(httpContext, ex, HttpStatusCode.InternalServerError).ConfigureAwait(false);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception, HttpStatusCode statusCode)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;
            await context.Response.WriteAsJsonAsync(exception.Message).ConfigureAwait(false);
        }
    }
}
