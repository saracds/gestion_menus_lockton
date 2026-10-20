using Lockton.Data.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Lockton.Data.Repositorios;

public class UsuarioRepositorio : IUsuarioRepositorio
{
    private readonly AppDbContext _db;

    public UsuarioRepositorio(AppDbContext db) => _db = db;

    public Task<List<Usuario>> ObtenerTodosAsync() =>
        _db.Usuarios.AsNoTracking().OrderBy(u => u.Id).ToListAsync();

    public Task<Usuario?> ObtenerPorIdAsync(int id) =>
        _db.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

    public Task<Usuario?> ObtenerPorNombreUsuarioAsync(string nombreUsuario) =>
        _db.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario);

    public Task<bool> ExisteNombreUsuarioAsync(string nombreUsuario) =>
        _db.Usuarios.AnyAsync(u => u.NombreUsuario == nombreUsuario);

    public async Task AgregarAsync(Usuario usuario)
    {
        _db.Usuarios.Add(usuario);
        await _db.SaveChangesAsync();
    }

    public Task<List<Menu>> ObtenerMenusAsync(int usuarioId) =>
        _db.UsuarioMenus.AsNoTracking()
            .Where(um => um.UsuarioId == usuarioId && um.Menu.Activo)
            .Select(um => um.Menu)
            .OrderBy(m => m.Id)
            .ToListAsync();

    public async Task ReemplazarMenusAsync(int usuarioId, IEnumerable<int> idsMenus)
    {
        var idsNuevos = idsMenus.Distinct().ToList();
        var actuales = await _db.UsuarioMenus.Where(um => um.UsuarioId == usuarioId).ToListAsync();

        // 1. Quitar los que ya no vienen en la lista
        _db.UsuarioMenus.RemoveRange(actuales.Where(um => !idsNuevos.Contains(um.MenuId)));

        // 2. Agregar los nuevos que aún no tenía
        var idsActuales = actuales.Select(um => um.MenuId).ToHashSet();
        foreach (var menuId in idsNuevos.Where(id => !idsActuales.Contains(id)))
        {
            _db.UsuarioMenus.Add(new UsuarioMenu
            {
                UsuarioId = usuarioId,
                MenuId = menuId,
                FechaAsignacion = DateTime.UtcNow
            });
        }

        // Un solo SaveChanges = una sola transacción: o se aplica todo o nada.
        await _db.SaveChangesAsync();
    }

    public Task<bool> TieneAccesoAMenuAsync(int usuarioId, string ruta) =>
        _db.UsuarioMenus.AnyAsync(um =>
            um.UsuarioId == usuarioId &&
            um.Menu.Ruta == ruta &&
            um.Menu.Activo &&
            um.Usuario.Activo);
}
