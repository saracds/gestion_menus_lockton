using Lockton.Data.Entidades;
using Lockton.Data.Repositorios;
using Lockton.Services.Dtos;
using Lockton.Services.Excepciones;
using Lockton.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Lockton.Services.Servicios;

/// <summary>Reglas de negocio de usuarios y de asignación de menús.</summary>
public class UsuarioServicio : IUsuarioServicio
{
    private readonly IUsuarioRepositorio _usuarios;
    private readonly IMenuRepositorio _menus;
    private readonly IPasswordHasher<Usuario> _hasher;

    public UsuarioServicio(IUsuarioRepositorio usuarios, IMenuRepositorio menus, IPasswordHasher<Usuario> hasher)
    {
        _usuarios = usuarios;
        _menus = menus;
        _hasher = hasher;
    }

    public async Task<List<UsuarioDto>> ObtenerTodosAsync() =>
        (await _usuarios.ObtenerTodosAsync()).Select(u => u.ADto()).ToList();

    public async Task<UsuarioDto> CrearAsync(CrearUsuarioDto datos)
    {
        var nombreUsuario = datos.NombreUsuario.Trim().ToLowerInvariant();

        if (await _usuarios.ExisteNombreUsuarioAsync(nombreUsuario))
            throw new ConflictoException($"El nombre de usuario '{nombreUsuario}' ya existe.");

        var usuario = new Usuario
        {
            NombreCompleto = datos.NombreCompleto.Trim(),
            NombreUsuario = nombreUsuario,
            Rol = datos.Rol,
            Activo = true,
            FechaCreacion = DateTime.UtcNow
        };

        // Hash + salt aleatorio. Lo que se guarda NO se puede revertir a la contraseña.
        usuario.ContrasenaHash = _hasher.HashPassword(usuario, datos.Contrasena);

        await _usuarios.AgregarAsync(usuario);
        return usuario.ADto();
    }

    public async Task<List<MenuDto>> ObtenerMenusAsync(int usuarioId)
    {
        await ValidarQueExisteAsync(usuarioId);
        return (await _usuarios.ObtenerMenusAsync(usuarioId)).Select(m => m.ADto()).ToList();
    }

    public async Task<List<MenuDto>> AsignarMenusAsync(int usuarioId, AsignarMenusDto datos)
    {
        await ValidarQueExisteAsync(usuarioId);

        var idsSolicitados = datos.IdsMenus.Distinct().ToList();
        var idsValidos = await _menus.ObtenerIdsActivosExistentesAsync(idsSolicitados);
        var idsInvalidos = idsSolicitados.Except(idsValidos).ToList();

        if (idsInvalidos.Count > 0)
            throw new ValidacionNegocioException(
                $"Los siguientes menús no existen o están inactivos: {string.Join(", ", idsInvalidos)}.");

        await _usuarios.ReemplazarMenusAsync(usuarioId, idsValidos);
        return await ObtenerMenusAsync(usuarioId);
    }

    private async Task ValidarQueExisteAsync(int usuarioId)
    {
        if (await _usuarios.ObtenerPorIdAsync(usuarioId) is null)
            throw new NoEncontradoException($"No existe el usuario con id {usuarioId}.");
    }
}
