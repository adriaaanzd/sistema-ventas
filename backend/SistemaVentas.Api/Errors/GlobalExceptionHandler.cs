using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace SistemaVentas.Api.Errors;

public class GlobalExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (status, title, detail) = exception switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, "Recurso no encontrado", exception.Message),
            ConflictException => (StatusCodes.Status409Conflict, "Conflicto con los datos existentes", exception.Message),
            BusinessRuleException => (StatusCodes.Status400BadRequest, "Regla de negocio no cumplida", exception.Message),
            // Respaldo si dos peticiones simultáneas pasan la validación del servicio: 2601/2627 = índice único, 547 = clave foránea o CHECK.
            DbUpdateException { InnerException: SqlException { Number: 2601 or 2627 } } =>
                (StatusCodes.Status409Conflict, "Conflicto con los datos existentes", "Ya existe un registro con ese valor único."),
            DbUpdateException { InnerException: SqlException { Number: 547 } } =>
                (StatusCodes.Status409Conflict, "Conflicto con los datos existentes", "La operación viola una relación o restricción de la base de datos."),
            _ => (StatusCodes.Status500InternalServerError, "Error interno del servidor", "Ocurrió un error inesperado. Intente nuevamente.")
        };

        if (status == StatusCodes.Status500InternalServerError)
            logger.LogError(exception, "Error no controlado en {Metodo} {Ruta}", httpContext.Request.Method, httpContext.Request.Path);

        httpContext.Response.StatusCode = status;
        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = { Status = status, Title = title, Detail = detail }
        });
    }
}
