using Microsoft.AspNetCore.Mvc;
using PortalProveedor.Authorization;
using PortalProveedor.Entities;
using PortalProveedor.Helpers;
using PortalProveedor.Models.Imputacion;
using PortalProveedor.Models.Proyectos;
using PortalProveedor.Services;

namespace PortalProveedor.Controllers
{
    [Authorize("Administrador/Contable")]
    [Route("api/[controller]")]
    [ApiController]
    public class ImputacionController : ControllerBase
    {
        private readonly ILogger<ImputacionController> _logger;
        private readonly IImputacionService _imputacionService;
        
        public ImputacionController(ILogger<ImputacionController> logger, IImputacionService imputacionService)
        {
            _logger = logger;
            _imputacionService = imputacionService;
        }

        /// <summary>
        /// Obtener Imputación.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ImputacionResponse>> GetbyId(int id)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());
            ImputacionResponse imputacion = await _imputacionService.GetById(id);
            return imputacion;
        }

        /// <summary>
        /// Obtener Lista de imputaciones.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ListaImputacionResponse>>> GetImputaciones()
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());
            IEnumerable<ListaImputacionResponse> response = await _imputacionService.GetImputaciones();
            return response.ToList();
        }

        /// <summary>
        /// Agregar nueva impuatación.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> AltaImputacion(AltaImputacionRequest dto)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());
            var usr = (Usuario)HttpContext.Items["usuario"];
            await _imputacionService.AltaImputacion(dto, usr.Id);
            return Ok(new { message = "Alta realizada correctamente" });
        }

        /// <summary>
        /// Actualizar impuatación.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult> ActualizarImputacion(int id, EditImputacionRequest dto)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());
            var usr = (Usuario)HttpContext.Items["usuario"];
            await _imputacionService.ActualizarImputacion(id, dto, usr.Id);
            return Ok(new { message = "Edición imputación correctamente" });
        }
    }
}
