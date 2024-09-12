using Microsoft.AspNetCore.Mvc;
using PortalProveedor.Authorization;
using PortalProveedor.Database;
using PortalProveedor.Entities;
using PortalProveedor.Helpers;
using PortalProveedor.Models.Proyectos;
using PortalProveedor.Models.Sociedades;
using PortalProveedor.Models.Usuarios;
using PortalProveedor.Services;
using System.Data;

namespace PortalProveedor.Controllers
{
    [Authorize("Administrador/Contable")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProyectoController : ControllerBase
    {
        private readonly PortalProveedorContext _context;
        private readonly ILogger<ProyectoController> _logger;
        private readonly IProyectoService _proyectoService;

        public ProyectoController(PortalProveedorContext context, ILogger<ProyectoController> logger, IProyectoService proyectoService)
        {
            _context = context;
            _logger = logger;
            _proyectoService = proyectoService;
        }

        /// <summary>
        /// Obtener Lista de Proyectos por Aprobador y Sociedad.
        /// </summary>
        [HttpGet]
        public ActionResult<IEnumerable<ListaProyectoResponse>> GetProyectos([FromQuery]BusquedaProyectoRequest request)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());

            var usr = (Usuario)HttpContext.Items["usuario"];
            if (usr is null) return BadRequest();

            IEnumerable<ListaProyectoResponse> response = _proyectoService.GetProyectos(usr.Id, usr.RolSociedadUsuarios.Select(s => s.Sociedad), request);
            return Ok(response);
        }

        /// <summary>
        /// Obtener Lista de Proyectos para Sociedad.
        /// </summary>
        [HttpGet("{sociedad}")]
        public ActionResult<IEnumerable<ListaProyectoResponse>> GetProyectosBySociedad(int sociedad)
        {
            IEnumerable<ListaProyectoResponse> response = _proyectoService.GetProyectosBySociedad(sociedad);
            return Ok(response);
        }

        [HttpGet("ByUsuario")]
        public ActionResult<IEnumerable<ListaProyectoResponse>> GetProyectosByUsuario()
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());

            var usr = (Usuario)HttpContext.Items["usuario"];
            if (usr is null) return BadRequest();
            IEnumerable<ListaProyectoResponse> response = _proyectoService.GetProyectosByUsuario(usr.Id);
            return Ok(response);
        }

        /// <summary>
        /// Lista todos los valores con sus ids de la tabla EstadoProyecto.
        /// </summary>
        [HttpGet("estados")]
        public ActionResult<IEnumerable<ListaEstadoProyectoResponse>> GetProyectosEstados()
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());

            var usr = (Usuario)HttpContext.Items["usuario"];
            if (usr is null) return BadRequest();

            IEnumerable<ListaEstadoProyectoResponse> response = _proyectoService.GetProyectosEstados(usr.Id, usr.RolSociedadUsuarios.Select(s => s.Sociedad));
            return Ok(response);
        }

        /// <summary>
        /// Crear un nuevo proyecto
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AltaProyecto(AltaProyectoRequest dto)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());

            var usr = (Usuario)HttpContext.Items["usuario"];
            if (usr is null) return BadRequest();

            await _proyectoService.AltaProyecto(usr.Id, dto);
            return Ok(new { message = "Alta realizada correctamente" });
        }

        /// <summary>
        /// Actualizar Proyecto.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProyecto(int id, AltaProyectoRequest dto)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());

            var usr = (Usuario)HttpContext.Items["usuario"];
            if (usr is null) return BadRequest();

            await _proyectoService.ActualizarProyecto(id, usr.Id, dto);
            return Ok(new { message = "Proyecto actualizado correctamente" });
        }

        /// <summary>
        /// Eliminar Proyecto.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProyecto(int id)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());

            var usr = (Usuario)HttpContext.Items["usuario"];
            if (usr is null) return BadRequest();

            await _proyectoService.EliminarProyecto(id, usr.Id);            
            return Ok(new { message = "Proyecto eliminado correctamente " });
        }
    }
}
