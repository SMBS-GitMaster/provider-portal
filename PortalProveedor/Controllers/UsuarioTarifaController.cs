using Microsoft.AspNetCore.Mvc;
using PortalProveedor.Authorization;
using PortalProveedor.Helpers;
using PortalProveedor.Models.UsuarioTarifa;
using PortalProveedor.Services;

namespace PortalProveedor.Controllers
{
    [Authorize("Administrador/Contable")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioTarifaController : ControllerBase
    {
        private readonly ILogger<ImputacionController> _logger;
        private readonly IUsuarioTarifaService _usuarioTarifaService;

        public UsuarioTarifaController(ILogger<ImputacionController> logger, IUsuarioTarifaService usuarioTarifaService)
        {
            _logger = logger;
            _usuarioTarifaService = usuarioTarifaService;
        }

        /// <summary>
        /// Crear Tarifa.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> AltaUsuarioTarifa(AltaUsuarioTarifaRequest dto)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());
            await _usuarioTarifaService.AltaUsuarioTarifa(dto);
            return Ok(new { message = "Alta realizada correctamente" });
        }

        /// <summary>
        /// Actualizar Tarifa.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult> ActualizarUsuarioTarifa(int id, EditUsuarioTarifaRequest dto)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());
            await _usuarioTarifaService.ActualizarUsuarioTarifa(id, dto);
            return Ok(new { message = "Edición Usuario tarifa correctamente" });
        }

        /// <summary>
        /// Eliminar Tarifa.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarUsuarioTarifa(int id)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());
            await _usuarioTarifaService.EliminarUsuarioTarifa(id);
            return Ok(new { message = "Tarifa del usuario eliminado correctamente " });
        }

        /// <summary>
        /// Obtener Lista de Tarifas.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioTarifaResponse>>> GetUsuarioTarifas()
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());
            IEnumerable<UsuarioTarifaResponse> tarifaResponse = await _usuarioTarifaService.GetUsuarioTarifas();
            return tarifaResponse.ToList();
        }

        /// <summary>
        /// Obtener Tarifa.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioTarifaResponse>> GetbyId(int id)
        {
            return await _usuarioTarifaService.GetById(id);
        }

    }
}
