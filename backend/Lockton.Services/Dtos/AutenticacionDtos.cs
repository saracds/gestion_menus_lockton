using System.ComponentModel.DataAnnotations;

namespace Lockton.Services.Dtos;

// DTO = objeto que viaja por la API (lo que se recibe y lo que se devuelve).
// Nunca se exponen las entidades: así el hash de la contraseña jamás sale en una respuesta.

/// <summary>Datos de entrada para iniciar sesión.</summary>
public class LoginDto
{
    [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
    [StringLength(50, ErrorMessage = "El nombre de usuario no puede superar 50 caracteres.")]
    public string NombreUsuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(100, ErrorMessage = "La contraseña no puede superar 100 caracteres.")]
    public string Contrasena { get; set; } = string.Empty;
}

/// <summary>Respuesta de un inicio de sesión exitoso.</summary>
public class LoginRespuestaDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiraEn { get; set; }
    public UsuarioDto Usuario { get; set; } = null!;
}
