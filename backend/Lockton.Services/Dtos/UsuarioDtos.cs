using System.ComponentModel.DataAnnotations;

namespace Lockton.Services.Dtos;

/// <summary>Usuario que devuelve la API</summary>
public class UsuarioDto
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string NombreUsuario { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public DateTime FechaCreacion { get; set; }
}

/// <summary>Datos para crear un usuario.</summary>
public class CrearUsuarioDto
{
    [Required(ErrorMessage = "El nombre completo es obligatorio.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre completo debe tener entre 3 y 100 caracteres.")]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre de usuario debe tener entre 3 y 50 caracteres.")]
    [RegularExpression("^[a-zA-Z0-9._-]+$", ErrorMessage = "El nombre de usuario solo admite letras sin tilde, números, punto, guion y guion bajo (sin espacios).")]
    public string NombreUsuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 100 caracteres.")]
    [RegularExpression("^(?=.*[A-Za-z])(?=.*\\d).+$", ErrorMessage = "La contraseña debe contener al menos una letra y un número.")]
    public string Contrasena { get; set; } = string.Empty;

    [Required(ErrorMessage = "El rol es obligatorio.")]
    [RegularExpression("^(Administrador|Usuario)$", ErrorMessage = "El rol debe ser 'Administrador' o 'Usuario'.")]
    public string Rol { get; set; } = "Usuario";
}

/// <summary>Lista completa de menús a asignar. Lista vacía = retirar todos.</summary>
public class AsignarMenusDto
{
    [Required(ErrorMessage = "La lista de menús es obligatoria (puede estar vacía para retirar todos).")]
    public List<int> IdsMenus { get; set; } = new();
}
