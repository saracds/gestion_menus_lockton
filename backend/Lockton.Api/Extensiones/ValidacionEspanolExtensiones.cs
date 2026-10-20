using Microsoft.AspNetCore.Mvc;

namespace Lockton.Api.Extensiones;

/// <summary>
/// Hace que TODAS las respuestas de validación salgan en español:
///  1. Mensajes de los DTOs → ya están en español en sus atributos [Required], [StringLength]...
///  2. Mensajes internos de ASP.NET al leer la petición (JSON mal formado, tipo de dato
///     incorrecto, cuerpo vacío) → se traducen aquí.
///  3. La respuesta 400 completa (título y errores) → se construye aquí en español.
/// </summary>
public static class ValidacionEspanolExtensiones
{
    private const string MensajeFormatoInvalido = "El formato del dato enviado no es válido.";

    public static IServiceCollection AgregarControladoresConValidacionEnEspanol(this IServiceCollection servicios)
    {
        servicios
            .AddControllers(opciones =>
            {
                // Evita el mensaje automático en inglés "The X field is required."
                // que .NET agrega a las propiedades no anulables.
                opciones.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;

                var mensajes = opciones.ModelBindingMessageProvider;
                mensajes.SetMissingRequestBodyRequiredValueAccessor(() => "El cuerpo de la petición es obligatorio.");
                mensajes.SetMissingBindRequiredValueAccessor(campo => $"No se envió un valor para '{campo}'.");
                mensajes.SetMissingKeyOrValueAccessor(() => "Se requiere un valor.");
                mensajes.SetValueMustNotBeNullAccessor(valor => $"El valor '{valor}' no puede ser nulo.");
                mensajes.SetAttemptedValueIsInvalidAccessor((valor, campo) => $"El valor '{valor}' no es válido para '{campo}'.");
                mensajes.SetNonPropertyAttemptedValueIsInvalidAccessor(valor => $"El valor '{valor}' no es válido.");
                mensajes.SetUnknownValueIsInvalidAccessor(campo => $"El valor enviado para '{campo}' no es válido.");
                mensajes.SetNonPropertyUnknownValueIsInvalidAccessor(() => "El valor enviado no es válido.");
                mensajes.SetValueIsInvalidAccessor(valor => $"El valor '{valor}' no es válido.");
                mensajes.SetValueMustBeANumberAccessor(campo => $"El campo '{campo}' debe ser un número.");
                mensajes.SetNonPropertyValueMustBeANumberAccessor(() => "El campo debe ser un número.");
            })
            .AddJsonOptions(opciones =>
            {
                // No exponer los mensajes técnicos (en inglés) del lector de JSON.
                opciones.AllowInputFormatterExceptionMessages = false;
            })
            .ConfigureApiBehaviorOptions(opciones =>
            {
                // Respuesta 400 automática de [ApiController], en español.
                opciones.InvalidModelStateResponseFactory = contexto =>
                {
                    var errores = contexto.ModelState
                        .Where(entrada => entrada.Value?.Errors.Count > 0)
                        .ToDictionary(
                            entrada => entrada.Key,
                            entrada => entrada.Value!.Errors
                                .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? MensajeFormatoInvalido : e.ErrorMessage)
                                .ToArray());

                    var problema = new ValidationProblemDetails(errores)
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Datos inválidos",
                        Detail = "Uno o más campos no cumplen las validaciones."
                    };

                    return new BadRequestObjectResult(problema);
                };
            });

        return servicios;
    }
}
