using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace University.Api.Common.Errors;

/// <summary>
/// Turns database failures into Problem Details. Endpoints check for duplicates first so they can
/// return a friendly message; this handler covers the races between that check and the save.
/// Any other exception becomes a 500 that reveals nothing about the server.
/// Since .NET 10 the middleware no longer logs exceptions that a handler handles, so this one
/// logs them itself.
/// </summary>
internal sealed partial class DbExceptionHandler(
    IProblemDetailsService problemDetails, ILogger<DbExceptionHandler> logger) : IExceptionHandler
{
    private const int UniqueIndexViolation = 2601;
    private const int UniqueConstraintViolation = 2627;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problem = exception switch
        {
            DbUpdateConcurrencyException => new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "The record was changed by someone else.",
                Detail = "Reload it and try again.",
            },
            _ when IsUniqueViolation(exception) => new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "A record with the same value already exists.",
            },
            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An unexpected error occurred.",
            },
        };

        if (problem.Status == StatusCodes.Status409Conflict)
        {
            LogConflict(logger, exception);
        }
        else
        {
            LogUnexpected(logger, exception);
        }

        httpContext.Response.StatusCode = problem.Status!.Value;
        return await problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problem,
        });
    }

    // SaveChanges wraps the SqlException in a DbUpdateException; ExecuteUpdate/ExecuteDelete and
    // raw SQL throw it directly.
    private static bool IsUniqueViolation(Exception exception) =>
        (exception as SqlException ?? exception.InnerException as SqlException) is
        { Number: UniqueIndexViolation or UniqueConstraintViolation };

    [LoggerMessage(Level = LogLevel.Warning, Message = "Database conflict, returning 409")]
    private static partial void LogConflict(ILogger logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "Unhandled exception, returning 500")]
    private static partial void LogUnexpected(ILogger logger, Exception exception);
}
