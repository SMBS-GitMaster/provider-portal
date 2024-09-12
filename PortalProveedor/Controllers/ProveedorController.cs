using Microsoft.AspNetCore.Mvc;
using PortalProveedor.Authorization;
using PortalProveedor.Database;
using PortalProveedor.Entities;
using PortalProveedor.Helpers;
using PortalProveedor.Models.Facturas;
using PortalProveedor.Models.Proveedores;
using PortalProveedor.Models.Proyectos;
using PortalProveedor.Models.Sociedades;
using PortalProveedor.Services;

namespace PortalProveedor.Controllers
{
    [Authorize("Administrador/Contable")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProveedorController : ControllerBase
    {
        private readonly PortalProveedorContext _context;
        private readonly ILogger<ProveedorController> _logger;
        private readonly IProveedorService _proveedorService;


        public ProveedorController(PortalProveedorContext context, ILogger<ProveedorController> logger, IProveedorService proveedorService)
        {
            _context = context;
            _logger = logger;
            _proveedorService=proveedorService;
        }

        /// <summary>
        /// Obtener Proveedor.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProveedorResponse>> GetProveedor(int id)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());

            string usrtype = (string)HttpContext.Items["usrtype"];
            if (string.IsNullOrEmpty(usrtype)) return BadRequest();

            switch (usrtype)
            {
                case "proveedor":
                    var proveedor = (Proveedor)HttpContext.Items["proveedor"];
                    if (proveedor is null) return BadRequest();

                    ProveedorResponse response = await _proveedorService.GetProveedor(proveedor.Id, usrtype, id);
                    if (response is null) return NotFound();
                    return response;
                default:
                    var usr = (Usuario)HttpContext.Items["usuario"];
                    if (usr is null) return BadRequest();

                    response = await _proveedorService.GetProveedor(usr.Id, usrtype, id);
                    if (response is null) return NotFound();
                    return response;
            }
        }

        /// <summary>
        /// Obtener Lista de Proveedores.
        /// </summary>
        [HttpGet("ByUsuario")]
        public ActionResult<IEnumerable<ProveedorResponse>> GetProveedoresByUsuario([FromQuery] BusquedaProveedorRequest request)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());

            var usr = (Usuario)HttpContext.Items["usuario"];
            if (usr is null) return BadRequest();

            IEnumerable<ProveedorResponse> response = _proveedorService.GetProveedoresByUsr(usr.Id);
            return Ok(response);
        }

        /// <summary>
        /// Obtener Lista de Proveedores.
        /// </summary>
        [HttpGet]
        public ActionResult<IEnumerable<ProveedorResponse>> GetProveedores([FromQuery] BusquedaProveedorRequest request)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());

            var usr = (Usuario)HttpContext.Items["usuario"];
            if (usr is null) return BadRequest();

            IEnumerable<ProveedorResponse> response = _proveedorService.GetProveedores(usr.Id, usr.RolSociedadUsuarios.Select(s => s.Sociedad), request);
            return Ok(response);
        }

        /// <summary>
        /// Lista con todos los valores con sus ids de la tabla EstadoProveedor.
        /// </summary>
        [HttpGet("estados")]
        public ActionResult<IEnumerable<ListaEstadoProveedorResponse>> GetProveedoresEstados()
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());

            var usr = (Usuario)HttpContext.Items["usuario"];
            if (usr is null) return BadRequest();

            IEnumerable<ListaEstadoProveedorResponse> response = _proveedorService.GetProveedoresEstados(usr.Id, usr.RolSociedadUsuarios.Select(s => s.Sociedad));
            return Ok(response);
        }

        /// <summary>
        /// Agregar nuevo Proveedor.
        /// </summary>
        [HttpPost("alta-proveedor")]
        public async Task<IActionResult> AltaProveedorAsync(AltaProveedorRequest dto)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());

            var usr = (Usuario)HttpContext.Items["usuario"];
            if (usr is null) return BadRequest();

            await _proveedorService.AltaProveedor(usr.Id, dto);
            return Ok(new { message = "Alta realizada correctamente" });
        }

        /// <summary>
        /// Actualizar Proveedor.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProveedor(int id, EditProveedorRequest dto)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());

            var usr = (Usuario)HttpContext.Items["usuario"];
            if (usr is null) return BadRequest();

            await _proveedorService.ActualizarProveedor(id, usr.Id, dto);
            return NoContent();
        }

        /// <summary>
        /// Eliminar Proveedor.
        /// </summary>
        // DELETE: api/Proveedor/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProveedor(int id)
        {
            var proveedor = await _context.Proveedors.FindAsync(id);
            if (proveedor == null)
            {
                return NotFound();
            }

            _context.Proveedors.Remove(proveedor);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
