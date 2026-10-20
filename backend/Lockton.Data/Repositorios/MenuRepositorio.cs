using Lockton.Data.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Lockton.Data.Repositorios;

public class MenuRepositorio : IMenuRepositorio
{
    private readonly AppDbContext _db;

    public MenuRepositorio(AppDbContext db) => _db = db;

    public Task<List<Menu>> ObtenerActivosAsync() =>
        _db.Menus.AsNoTracking().Where(m => m.Activo).OrderBy(m => m.Id).ToListAsync();

    public Task<Menu?> ObtenerPorRutaAsync(string ruta) =>
        _db.Menus.AsNoTracking().FirstOrDefaultAsync(m => m.Ruta == ruta && m.Activo);

    public Task<List<int>> ObtenerIdsActivosExistentesAsync(IEnumerable<int> ids)
    {
        var listaIds = ids.Distinct().ToList();
        return _db.Menus.Where(m => listaIds.Contains(m.Id) && m.Activo)
                        .Select(m => m.Id)
                        .ToListAsync();
    }
}
