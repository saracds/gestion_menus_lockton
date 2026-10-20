namespace Lockton.Data.Entidades;

/// <summary>Opción de menú / módulo funcional. Mapea la tabla dbo.Menus.</summary>
public class Menu
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }

    /// <summary>Identificador funcional y ruta del módulo en el frontend (ej: "polizas").</summary>
    public string Ruta { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;

    public ICollection<UsuarioMenu> UsuarioMenus { get; set; } = new List<UsuarioMenu>();
}
