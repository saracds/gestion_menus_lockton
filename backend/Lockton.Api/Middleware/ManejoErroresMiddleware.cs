using Lockton.Services.Excepciones;
using Microsoft.AspNetCore.Mvc;

namespace Lockton.Api.Middleware;

/// <summary>
/// Captura cualquier excepción y la convierte en una respuesta JSON
/// (formato estándar ProblemDetails) con el código HTTP adecuado.
/// Así los controladores quedan limpios, sin try/catch.
/// </summary>
public class ManejoErroresMiddleware
{
    private readonly RequestDelegate _siguiente;
    private readonly ILogger<ManejoErroresMiddleware> _logger;

    public ManejoErroresMiddleware(RequestDelegate siguiente, ILogger<ManejoErroresMiddleware> logger)
    {
        _siguiente = siguiente;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext contexto)
    {
        try
        {
            await _siguiente(contexto);
        }
        catch (Exception ex)
        {
            var (estado, titulo) = ex switch
            {
                ValidacionNegocioException     => (StatusCodes.Status400BadRequest, "Datos inválidos"),
                CredencialesInvalidasException => (StatusCodes.Status401Unauthorized, "No autenticado"),
                AccesoDenegadoException        => (StatusCodes.Status403Forbidden, "Acceso denegado"),
                NoEncontradoException          => (StatusCodes.Status404NotFound, "No encontrado"),
                ConflictoException             => (StatusCodes.Status409Conflict, "Conflicto"),
                _                              => (StatusCodes.Status500InternalServerError, "Error interno")
            };

            // Errores inesperados: se registran completos en el log, pero al
            // cliente NO se le envían detalles internos (stack trace, SQL, etc.).
            if (estado == StatusCodes.Status500InternalServerError)
                _logger.LogError(ex, "Error no controlado");

            var problema = new ProblemDetails
            {
                Status = estado,
                Title = titulo,
                Detail = estado == StatusCodes.Status500InternalServerError
                    ? "Ocurrió un error inesperado. Intente más tarde."
                    : ex.Message
            };

            contexto.Response.StatusCode = estado;
            await contexto.Response.WriteAsJsonAsync(problema);
        }
    }
}
