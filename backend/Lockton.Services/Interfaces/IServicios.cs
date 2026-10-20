using Lockton.Data.Entidades;
using Lockton.Services.Dtos;

namespace Lockton.Services.Interfaces;

public interface IAutenticacionServicio
{
    Task<LoginRespuestaDto> IniciarSesionAsync(LoginDto datos);
}

public interface ITokenServicio
{
    (string token, DateTime expiraEn) CrearToken(Usuario usuario);
}

public interface IUsuarioServicio
{
    Task<List<UsuarioDto>> ObtenerTodosAsync();
    Task<UsuarioDto> CrearAsync(CrearUsuarioDto datos);
    Task<List<MenuDto>> ObtenerMenusAsync(int usuarioId);
    Task<List<MenuDto>> AsignarMenusAsync(int usuarioId, AsignarMenusDto datos);
}

public interface IMenuServicio
{
    Task<List<MenuDto>> ObtenerTodosAsync();
    Task<ModuloDto> ObtenerModuloAsync(int usuarioId, string ruta);
}
