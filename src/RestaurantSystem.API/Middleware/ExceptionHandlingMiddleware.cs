using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Domain.Common;
using System.Net;

namespace RestaurantSystem.API.Middleware
{
    public sealed class ExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
            => _logger = logger;

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (DomainException ex)
            {
                await WriteProblem(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                await WriteProblem(context, HttpStatusCode.NotFound, ex.Message);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning(ex, "Concurrency conflict");
                await WriteProblem(context, HttpStatusCode.Conflict, "Conflicto de concurrencia. Reintenta la operación.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error");
                await WriteProblem(context, HttpStatusCode.InternalServerError, "Error interno del servidor.");
            }
        }

        private static async Task WriteProblem(HttpContext ctx, HttpStatusCode code, string detail)
        {
            ctx.Response.StatusCode = (int)code;
            ctx.Response.ContentType = "application/problem+json";

            var problem = new ProblemDetails
            {
                Status = (int)code,
                Title = code.ToString(),
                Detail = detail
            };

            await ctx.Response.WriteAsJsonAsync(problem);
        }
    }
}
