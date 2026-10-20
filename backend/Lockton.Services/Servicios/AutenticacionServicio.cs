using Lockton.Data.Entidades;
using Lockton.Data.Repositorios;
using Lockton.Services.Dtos;
using Lockton.Services.Excepciones;
using Lockton.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Lockton.Services.Servicios;

/// <summary>Autenticación: valida las credenciales y emite el token.</summary>
public class AutenticacionServicio : IAutenticacionServicio
{
    private readonly IUsuarioRepositorio _usuarios;
    private readonly IPasswordHasher<Usuario> _hasher;
    private readonly ITokenServicio _tokenServicio;

    public AutenticacionServicio(IUsuarioRepositorio usuarios, IPasswordHasher<Usuario> hasher, ITokenServicio tokenServicio)
    {
        _usuarios = usuarios;
        _hasher = hasher;
        _tokenServicio = tokenServicio;
    }

    public async Task<LoginRespuestaDto> IniciarSesionAsync(LoginDto datos)
    {
        var usuario = await _usuarios.ObtenerPorNombreUsuarioAsync(datos.NombreUsuario.Trim());

        // Mensaje genérico: no revelamos si falló el usuario o la contraseña.
        if (usuario is null || !usuario.Activo)
            throw new CredencialesInvalidasException();

        // PasswordHasher extrae el salt del hash guardado y recalcula PBKDF2.
        var resultado = _hasher.VerifyHashedPassword(usuario, usuario.ContrasenaHash, datos.Contrasena);
        if (resultado == PasswordVerificationResult.Failed)
            throw new CredencialesInvalidasException();

        var (token, expiraEn) = _tokenServicio.CrearToken(usuario);

        return new LoginRespuestaDto { Token = token, ExpiraEn = expiraEn, Usuario = usuario.ADto() };
    }
}
