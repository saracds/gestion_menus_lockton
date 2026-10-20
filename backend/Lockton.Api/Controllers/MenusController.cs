using Lockton.Services.Dtos;
using Lockton.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lockton.Api.Controllers;

[ApiController]
[Route("api/menus")]
[Authorize]
public class MenusController : ControllerBase
{
    private readonly IMenuServicio _menuServicio;

    public MenusController(IMenuServicio menuServicio) => _menuServicio = menuServicio;

    // GET /api/menus → catálogo de menús activos (cualquier usuario autenticado)
    [HttpGet]
    public async Task<ActionResult<List<MenuDto>>> Listar() =>
        Ok(await _menuServicio.ObtenerTodosAsync());
}
