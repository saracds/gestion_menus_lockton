using Lockton.Api.Extensiones;
using Lockton.Services.Dtos;
using Lockton.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lockton.Api.Controllers;

[ApiController]
[Route("api/modules")]
[Authorize]
public class ModulosController : ControllerBase
{
    private readonly IMenuServicio _menuServicio;

    public ModulosController(IMenuServicio menuServicio) => _menuServicio = menuServicio;

    // GET /api/modules/{ruta} → 200 si tiene el menú asignado | 403 si no | 404 si no existe
    [HttpGet("{ruta}")]
    public async Task<ActionResult<ModuloDto>> Obtener(string ruta) =>
        Ok(await _menuServicio.ObtenerModuloAsync(User.ObtenerUsuarioId(), ruta));
}
