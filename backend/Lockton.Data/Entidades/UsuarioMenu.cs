namespace Lockton.Data.Entidades;

/// <summary>Asignación usuario-menú (relación muchos a muchos). Tabla dbo.UsuarioMenus.</summary>
public class UsuarioMenu
{
    public int UsuarioId { get; set; }
    public int MenuId { get; set; }
    public DateTime FechaAsignacion { get; set; }

    public Usuario Usuario { get; set; } = null!;
    public Menu Menu { get; set; } = null!;
}
