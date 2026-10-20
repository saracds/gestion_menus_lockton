using Lockton.Data.Repositorios;
using Lockton.Services.Dtos;
using Lockton.Services.Excepciones;
using Lockton.Services.Interfaces;

namespace Lockton.Services.Servicios;

/// <summary>Catálogo de menús y acceso a los módulos funcionales.</summary>
public class MenuServicio : IMenuServicio
{
    private readonly IMenuRepositorio _menus;
    private readonly IUsuarioRepositorio _usuarios;

    public MenuServicio(IMenuRepositorio menus, IUsuarioRepositorio usuarios)
    {
        _menus = menus;
        _usuarios = usuarios;
    }

    public async Task<List<MenuDto>> ObtenerTodosAsync() =>
        (await _menus.ObtenerActivosAsync()).Select(m => m.ADto()).ToList();

    public async Task<ModuloDto> ObtenerModuloAsync(int usuarioId, string ruta)
    {
        var menu = await _menus.ObtenerPorRutaAsync(ruta)
                   ?? throw new NoEncontradoException($"El módulo '{ruta}' no existe.");

        // AUTORIZACIÓN EN EL BACKEND: se consulta la BD en cada petición,
        // así un menú retirado deja de funcionar de inmediato.
        if (!await _usuarios.TieneAccesoAMenuAsync(usuarioId, ruta))
            throw new AccesoDenegadoException($"No tiene permiso para acceder al módulo '{menu.Nombre}'.");

        return new ModuloDto
        {
            Nombre = menu.Nombre,
            Descripcion = menu.Descripcion,
            Ruta = menu.Ruta,
            Mensaje = $"Acceso concedido al módulo {menu.Nombre}. (Contenido de demostración)"
        };
    }
}
