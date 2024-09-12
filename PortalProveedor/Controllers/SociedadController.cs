using Microsoft.AspNetCore.Mvc;
using PortalProveedor.Authorization;
using PortalProveedor.Database;
using PortalProveedor.Entities;
using PortalProveedor.Helpers;
using PortalProveedor.Models.Sociedades;
using PortalProveedor.Services;

namespace PortalProveedor.Controllers
{
    [Authorize(new string[] { "Proveedor", "Administrador/Contable" })]
    [Route("api/[controller]")]
    [ApiController]
    public class SociedadController : ControllerBase
    {
        private readonly PortalProveedorContext _context;
        private readonly ILogger<SociedadController> _logger;
        private readonly ISociedadService _sociedadService;


        public SociedadController(PortalProveedorContext context, ILogger<SociedadController> logger, ISociedadService sociedadService)
        {
            _context = context;
            _logger = logger;
            _sociedadService=sociedadService;
        }

        /// <summary>
        /// Obtener Lista de Sociedades por Aprobador y Sociedad.
        /// </summary>
        [HttpGet]
        public ActionResult<IEnumerable<ListaSociedadResponse>> GetSociedades([FromQuery] BusquedaSociedadRequest request)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());

            var usr = (Usuario)HttpContext.Items["usuario"];
            if (usr is null) return BadRequest();

            IEnumerable<ListaSociedadResponse> response = _sociedadService.GetSociedades(usr.Id, usr.RolSociedadUsuarios.Select(s => s.Sociedad), request);
            return Ok(response);
        }

        /// <summary>
        /// Obtener Lista de Sociedades por Usuario.
        /// </summary>
        [HttpGet("ByUsuario")]
        public ActionResult<IEnumerable<ListaSociedadResponse>> GetSociedadesByUsuario()
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());

            var usr = (Usuario)HttpContext.Items["usuario"];
            if (usr is null) return BadRequest();

            IEnumerable<ListaSociedadResponse> response = _sociedadService.GetSociedadesResponseByUsuario(usr.Id);
            return Ok(response);
        }


        /// <summary>
        /// Agregar nueva Sociedad.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AltaSociedad(AltaSociedadRequest dto)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());

            var usr = (Usuario)HttpContext.Items["usuario"];
            if (usr is null) return BadRequest();

            await _sociedadService.AltaSociedad(usr.Id, dto);
            return Ok(new { message = "Alta realizada correctamente" });
        }

        /// <summary>
        /// Actualizar Sociedad.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSociedad(int id, AltaSociedadRequest dto)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());

            var usr = (Usuario)HttpContext.Items["usuario"];
            if (usr is null) return BadRequest();

            await _sociedadService.ActualizarSociedad(id, usr.Id, dto);
            return NoContent();
        }

        /// <summary>
        /// Eliminar Sociedad.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSociedad(int id)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());

            var usr = (Usuario)HttpContext.Items["usuario"];
            if (usr is null) return BadRequest();

            await _sociedadService.EliminarSociedad(id, usr.Id);
            return Ok(new { message = "Sociedad eliminada correctamente " });
        }
    }
}
