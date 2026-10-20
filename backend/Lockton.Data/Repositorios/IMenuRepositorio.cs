using Lockton.Data.Entidades;

namespace Lockton.Data.Repositorios;

/// <summary>Operaciones de base de datos sobre el catálogo de menús.</summary>
public interface IMenuRepositorio
{
    Task<List<Menu>> ObtenerActivosAsync();
    Task<Menu?> ObtenerPorRutaAsync(string ruta);

    /// <summary>De los ids recibidos, devuelve solo los que existen y están activos.</summary>
    Task<List<int>> ObtenerIdsActivosExistentesAsync(IEnumerable<int> ids);
}
