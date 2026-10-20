namespace Lockton.Services.Dtos;

/// <summary>Menú que devuelve la API.</summary>
public class MenuDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string Ruta { get; set; } = string.Empty;
    public bool Activo { get; set; }
}

/// <summary>Contenido de un módulo funcional (simulado), protegido por la asignación de menús.</summary>
public class ModuloDto
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string Ruta { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
}
