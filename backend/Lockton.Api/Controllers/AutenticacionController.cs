using Lockton.Services.Dtos;
using Lockton.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lockton.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AutenticacionController : ControllerBase
{
    private readonly IAutenticacionServicio _autenticacionServicio;

    public AutenticacionController(IAutenticacionServicio autenticacionServicio) =>
        _autenticacionServicio = autenticacionServicio;

    // POST /api/auth/login → 200 con token | 400 datos faltantes | 401 credenciales incorrectas
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginRespuestaDto>> IniciarSesion([FromBody] LoginDto datos) =>
        Ok(await _autenticacionServicio.IniciarSesionAsync(datos));
}
