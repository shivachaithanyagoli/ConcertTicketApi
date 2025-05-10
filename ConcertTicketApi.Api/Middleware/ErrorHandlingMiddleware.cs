using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;
using FluentValidation;

namespace ConcertTicketApi.Api.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        public ErrorHandlingMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext ctx)
        {
            try
            {
                await _next(ctx);
            }
            catch (ValidationException ex)
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await ctx.Response.WriteAsJsonAsync(new { errors = ex.Errors });
            }
            catch (Exception ex)
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await ctx.Response.WriteAsJsonAsync(new {
                    error = "An unexpected error occurred.",
                    detail = ex.Message
                });
            }
        }
    }
}
