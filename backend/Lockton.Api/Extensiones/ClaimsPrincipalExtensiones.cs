using System.Security.Claims;
using Lockton.Services.Servicios;

namespace Lockton.Api.Extensiones;

/// <summary>Lee datos del usuario autenticado desde los claims del JWT.</summary>
public static class ClaimsPrincipalExtensiones
{
    public static int ObtenerUsuarioId(this ClaimsPrincipal usuario) =>
        int.Parse(usuario.FindFirstValue(TokenServicio.ClaimId)
                  ?? throw new UnauthorizedAccessException("El token no contiene el id del usuario."));
}
