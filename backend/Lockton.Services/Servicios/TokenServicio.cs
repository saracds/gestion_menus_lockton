using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Lockton.Data.Entidades;
using Lockton.Services.Configuracion;
using Lockton.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Lockton.Services.Servicios;

/// <summary>Genera el JWT firmado con HMAC-SHA256 que identifica al usuario.</summary>
public class TokenServicio : ITokenServicio
{
    // Nombres de los claims (datos dentro del token) usados en todo el backend
    public const string ClaimId = "id";
    public const string ClaimNombreUsuario = "nombreUsuario";
    public const string ClaimNombreCompleto = "nombreCompleto";
    public const string ClaimRol = "rol";

    private readonly ConfiguracionJwt _configuracion;

    public TokenServicio(IOptions<ConfiguracionJwt> configuracion) => _configuracion = configuracion.Value;

    public (string token, DateTime expiraEn) CrearToken(Usuario usuario)
    {
        var claims = new List<Claim>
        {
            new(ClaimId, usuario.Id.ToString()),
            new(ClaimNombreUsuario, usuario.NombreUsuario),
            new(ClaimNombreCompleto, usuario.NombreCompleto),
            new(ClaimRol, usuario.Rol)
        };

        var clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuracion.Clave));
        var credenciales = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256);
        var expiraEn = DateTime.UtcNow.AddMinutes(_configuracion.MinutosExpiracion);

        var jwt = new JwtSecurityToken(
            issuer: _configuracion.Emisor,
            audience: _configuracion.Audiencia,
            claims: claims,
            expires: expiraEn,
            signingCredentials: credenciales);

        return (new JwtSecurityTokenHandler().WriteToken(jwt), expiraEn);
    }
}
