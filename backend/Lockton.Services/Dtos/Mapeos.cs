using Lockton.Data.Entidades;

namespace Lockton.Services.Dtos;

/// <summary>Conversión Entidad → DTO (mapeo manual, simple y explícito).</summary>
public static class Mapeos
{
    public static UsuarioDto ADto(this Usuario u) => new()
    {
        Id = u.Id,
        NombreCompleto = u.NombreCompleto,
        NombreUsuario = u.NombreUsuario,
        Rol = u.Rol,
        Activo = u.Activo,
        FechaCreacion = u.FechaCreacion
    };

    public static MenuDto ADto(this Menu m) => new()
    {
        Id = m.Id,
        Nombre = m.Nombre,
        Descripcion = m.Descripcion,
        Ruta = m.Ruta,
        Activo = m.Activo
    };
}
