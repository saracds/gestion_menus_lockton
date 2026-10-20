using Lockton.Api.Extensiones;
using Lockton.Data.Entidades;
using Lockton.Services.Dtos;
using Lockton.Services.Excepciones;
using Lockton.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lockton.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize] // Todo el controlador exige un token válido
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioServicio _usuarioServicio;

    public UsuariosController(IUsuarioServicio usuarioServicio) => _usuarioServicio = usuarioServicio;

    // GET /api/users → solo Administrador
    [HttpGet]
    [Authorize(Roles = Roles.Administrador)]
    public async Task<ActionResult<List<UsuarioDto>>> Listar() =>
        Ok(await _usuarioServicio.ObtenerTodosAsync());

    // POST /api/users → solo Administrador | 201 creado | 400 inválido | 409 usuario duplicado
    [HttpPost]
    [Authorize(Roles = Roles.Administrador)]
    public async Task<ActionResult<UsuarioDto>> Crear([FromBody] CrearUsuarioDto datos)
    {
        var creado = await _usuarioServicio.CrearAsync(datos);
        return CreatedAtAction(nameof(ObtenerMenus), new { id = creado.Id }, creado);
    }

    // GET /api/users/{id}/menus → Administrador: cualquier usuario | Usuario: solo los suyos
    [HttpGet("{id:int}/menus")]
    public async Task<ActionResult<List<MenuDto>>> ObtenerMenus(int id)
    {
        // Autorización a nivel de recurso: un usuario normal no puede ver menús ajenos.
        if (!User.IsInRole(Roles.Administrador) && User.ObtenerUsuarioId() != id)
            throw new AccesoDenegadoException("Solo puede consultar sus propios menús.");

        return Ok(await _usuarioServicio.ObtenerMenusAsync(id));
    }

    // PUT /api/users/{id}/menus → solo Administrador. Reemplaza la lista completa (vacía = retirar todos)
    [HttpPut("{id:int}/menus")]
    [Authorize(Roles = Roles.Administrador)]
    public async Task<ActionResult<List<MenuDto>>> AsignarMenus(int id, [FromBody] AsignarMenusDto datos) =>
        Ok(await _usuarioServicio.AsignarMenusAsync(id, datos));
}
