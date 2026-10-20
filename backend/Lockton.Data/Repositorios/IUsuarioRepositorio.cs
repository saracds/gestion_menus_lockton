using Lockton.Data.Entidades;

namespace Lockton.Data.Repositorios;

/// <summary>Operaciones de base de datos sobre usuarios y sus menús.</summary>
public interface IUsuarioRepositorio
{
    Task<List<Usuario>> ObtenerTodosAsync();
    Task<Usuario?> ObtenerPorIdAsync(int id);
    Task<Usuario?> ObtenerPorNombreUsuarioAsync(string nombreUsuario);
    Task<bool> ExisteNombreUsuarioAsync(string nombreUsuario);
    Task AgregarAsync(Usuario usuario);

    /// <summary>Menús activos asignados al usuario.</summary>
    Task<List<Menu>> ObtenerMenusAsync(int usuarioId);

    /// <summary>Reemplaza el conjunto completo de menús del usuario.</summary>
    Task ReemplazarMenusAsync(int usuarioId, IEnumerable<int> idsMenus);

    /// <summary>¿El usuario (activo) tiene asignado el menú (activo) con esa ruta?</summary>
    Task<bool> TieneAccesoAMenuAsync(int usuarioId, string ruta);
}
