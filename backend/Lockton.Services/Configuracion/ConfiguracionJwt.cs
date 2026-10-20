namespace Lockton.Services.Configuracion;

/// <summary>Configuración del token JWT (sección "Jwt" de appsettings / variables de entorno Jwt__*).</summary>
public class ConfiguracionJwt
{
    /// <summary>Clave secreta para firmar (mínimo 32 caracteres). Variable de entorno: Jwt__Clave.</summary>
    public string Clave { get; set; } = string.Empty;
    public string Emisor { get; set; } = "Lockton.Api";
    public string Audiencia { get; set; } = "Lockton.Web";
    public int MinutosExpiracion { get; set; } = 60;
}
