namespace Lockton.Data.Entidades;

/// <summary>Usuario del sistema. Mapea la tabla dbo.Usuarios.</summary>
public class Usuario
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string NombreUsuario { get; set; } = string.Empty;

    /// <summary>Hash + salt (PBKDF2). Nunca se guarda la contraseña en texto plano.</summary>
    public string ContrasenaHash { get; set; } = string.Empty;

    /// <summary>"Administrador" o "Usuario".</summary>
    public string Rol { get; set; } = Roles.Usuario;
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; }

    public ICollection<UsuarioMenu> UsuarioMenus { get; set; } = new List<UsuarioMenu>();
}

/// <summary>
/// Valores permitidos para la columna Rol (ver CK_Usuarios_Rol en init.sql).
/// Constantes para no repetir textos "mágicos" en el código.
/// </summary>
public static class Roles
{
    public const string Administrador = "Administrador";
    public const string Usuario = "Usuario";
}
