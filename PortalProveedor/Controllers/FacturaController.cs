using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalProveedor.Authorization;
using PortalProveedor.Database;
using PortalProveedor.Entities;
using PortalProveedor.Helpers;
using PortalProveedor.Models.Dtos;
using PortalProveedor.Models.Facturas;
using PortalProveedor.Services;
using System.Data;

namespace PortalProveedor.Controllers
{
    [Authorize(new string[] { "Proveedor", "Administrador/Contable" })]
    [Route("api/[controller]")]
    [ApiController]
    public class FacturaController : ControllerBase
    {
        private readonly PortalProveedorContext _context;
        private readonly ILogger<FacturaController> _logger;
        private readonly IFacturaService _facturaService;

        public FacturaController(PortalProveedorContext context, ILogger<FacturaController> logger, IFacturaService facturaService)
        {
            _context = context;
            _logger = logger;
            _facturaService = facturaService;
        }

        /// <summary>
        /// Obtener Factura.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<FacturaResponse>> GetFactura(int id)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());

            string usrtype = (string)HttpContext.Items["usrtype"];
            if (string.IsNullOrEmpty(usrtype)) return BadRequest();

            switch (usrtype)
            {
                case "proveedor":
                    var proveedor = (Proveedor)HttpContext.Items["proveedor"];
                    if (proveedor is null) return BadRequest();

                    FacturaResponse response = await _facturaService.GetFactura(proveedor.Id, usrtype, id);
                    if (response is null) return NotFound();
                    return response;
                default:
                    var usr = (Usuario)HttpContext.Items["usuario"];
                    if (usr is null) return BadRequest();

                    response = await _facturaService.GetFactura(usr.Id, usrtype, id);
                    if (response is null) return NotFound();
                    return response;
            }
        }

        /// <summary>
        /// Obtener Lista de Facturas mediante parámetros de búsqueda
        /// </summary>
        [HttpPost("search")]
        public async Task<ActionResult<IEnumerable<FacturaResponse>>> GetFacturasSearch([FromBody] FiltrosBusquedaResponse filtros)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());

            string usrtype = (string)HttpContext.Items["usrtype"];
            if (string.IsNullOrEmpty(usrtype)) return BadRequest();
            
            switch (usrtype)
            {
                case "proveedor":
                    var proveedor = (Proveedor)HttpContext.Items["proveedor"];
                    if (proveedor is null) return BadRequest();
                    if (!_context.RolSociedadUsuarios.Any(a => a.Usuario == proveedor.Id && a.Rol == 2)) return BadRequest();
                    IEnumerable<FacturaResponse> response = await _facturaService.GetFacturasSearch(proveedor.Id, usrtype, filtros.Proveedores, filtros.Sociedades, filtros.Proyectos, filtros.Estados, filtros.ResponsablesActuales, filtros.Years);
                    return response.ToList();
                default:
                    var usr = (Usuario)HttpContext.Items["usuario"];
                    if (usr is null) return BadRequest();
                    if (!_context.RolSociedadUsuarios.Any(a => a.Usuario == usr.Id && a.Rol == 2)) return BadRequest();
                    response = await _facturaService.GetFacturasSearch(usr.Id, usrtype, filtros.Proveedores, filtros.Sociedades, filtros.Proyectos, filtros.Estados, filtros.ResponsablesActuales, filtros.Years);
                    return response.ToList();
            }
        }

        [HttpGet("GetYears")]
        public async Task<ActionResult<IEnumerable<int>>> GetYears()
        {
            string usrtype = (string)HttpContext.Items["usrtype"];
            if (string.IsNullOrEmpty(usrtype)) return BadRequest();

            switch (usrtype)
            {
                case "proveedor":
                    var proveedor = (Proveedor)HttpContext.Items["proveedor"];
                    if (proveedor is null) return BadRequest();

                    var response = await _facturaService.GetYears(proveedor.Id, usrtype);
                    return response.ToList();
                default:
                    var usr = (Usuario)HttpContext.Items["usuario"];
                    if (usr is null) return BadRequest();

                    response = await _facturaService.GetYears(usr.Id, usrtype);
                    return response.ToList();
            }
        }

        /// <summary>
        /// Obtener Lista de estados posibles
        /// </summary>
        [HttpGet("GetEstados")]
        public async Task<ActionResult<IEnumerable<EstadoFactura>>> GetEstados()
        {
            string usrtype = (string)HttpContext.Items["usrtype"];
            if (string.IsNullOrEmpty(usrtype)) return BadRequest();

            switch (usrtype)
            {
                case "proveedor":
                    var proveedor = (Proveedor)HttpContext.Items["proveedor"];
                    if (proveedor is null) return BadRequest();

                    var response = await _facturaService.GetEstados(proveedor.Id, usrtype);
                    return response.ToList();
                default:
                    var usr = (Usuario)HttpContext.Items["usuario"];
                    if (usr is null) return BadRequest();

                    response = await _facturaService.GetEstados(usr.Id, usrtype);
                    return response.ToList();
            }
        }

        /// <summary>
        /// Obtener Lista de Facturas por Estado.
        /// </summary>
        [HttpGet("estado-destino/{estado}")]
        public async Task<ActionResult<IEnumerable<FacturaResponse>>> GetFacturasByEstadoDestino(int estado)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());

            string usrtype = (string)HttpContext.Items["usrtype"];
            if (string.IsNullOrEmpty(usrtype)) return BadRequest();

            switch (usrtype)
            {
                case "proveedor":
                    var proveedor = (Proveedor)HttpContext.Items["proveedor"];
                    if (proveedor is null) return BadRequest();

                    IEnumerable<FacturaResponse> response = await _facturaService.GetFacturasByEstadoDestino(proveedor.Id, usrtype, estado);
                    return response.ToList();
                default:
                    var usr = (Usuario)HttpContext.Items["usuario"];
                    if (usr is null) return BadRequest();

                    response = await _facturaService.GetFacturasByEstadoDestino(usr.Id, usrtype, estado);
                    return response.ToList();
            }
        }

        /// <summary>
        /// Obtener Lista de Facturas por Estado.
        /// </summary>
        [HttpGet("estado/{estado}")]
        public async Task<ActionResult<IEnumerable<FacturaResponse>>> GetFacturasByEstado(int estado)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());

            string usrtype = (string)HttpContext.Items["usrtype"];
            if (string.IsNullOrEmpty(usrtype)) return BadRequest();

            switch (usrtype)
            {
                case "proveedor":
                    var proveedor = (Proveedor)HttpContext.Items["proveedor"];
                    if (proveedor is null) return BadRequest();

                    IEnumerable<FacturaResponse> response = await _facturaService.GetFacturasByEstado(proveedor.Id, usrtype, estado);
                    return response.ToList();
                default:
                    var usr = (Usuario)HttpContext.Items["usuario"];
                    if (usr is null) return BadRequest();

                    response = await _facturaService.GetFacturasByEstado(usr.Id, usrtype, estado);
                    return response.ToList();
            }
        }

        /// <summary>
        /// Obtener Lista de Facturas por pedido.
        /// </summary>
        [HttpGet("pedido/{pedido}")]
        public async Task<ActionResult<IEnumerable<FacturaResponse>>> GetFacturasByPedido(int pedido)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());

            string usrtype = (string)HttpContext.Items["usrtype"];
            if (string.IsNullOrEmpty(usrtype)) return BadRequest();

            switch (usrtype)
            {
                case "proveedor":
                    var proveedor = (Proveedor)HttpContext.Items["proveedor"];
                    if (proveedor is null) return BadRequest();

                    IEnumerable<FacturaResponse> response = await _facturaService.GetFacturasByPedido(proveedor.Id, usrtype, pedido);
                    return response.ToList();
                default:
                    var usr = (Usuario)HttpContext.Items["usuario"];
                    if (usr is null) return BadRequest();

                    response = await _facturaService.GetFacturasByPedido(usr.Id, usrtype, pedido);
                    return response.ToList();
            }
        }

        /// <summary>
        /// Agregar nueva Factura.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Factura>> AltaFactura([FromForm]AltaFacturaRequest dto)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());

            string usrtype = (string)HttpContext.Items["usrtype"];
            if (string.IsNullOrEmpty(usrtype)) return BadRequest();

            switch (usrtype)
            {
                case "proveedor":
                    var proveedor = (Proveedor)HttpContext.Items["proveedor"];
                    if (proveedor is null) return BadRequest();

                    await _facturaService.AltaFactura(proveedor.Id, usrtype, dto);
                    return Ok(new { message = "Alta factura correctamente" });
                default:
                    var usr = (Usuario)HttpContext.Items["usuario"];
                    if (usr is null) return BadRequest();

                    await _facturaService.AltaFactura(usr.Id, usrtype, dto);
                    return Ok(new { message = "Alta factura correctamente" });
            }
        }

        /// <summary>
        /// Cancelar Factura.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> CancelFactura(int id)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());

            string usrtype = (string)HttpContext.Items["usrtype"];
            if (string.IsNullOrEmpty(usrtype)) return BadRequest();

            switch (usrtype)
            {
                case "proveedor":
                    var proveedor = (Proveedor)HttpContext.Items["proveedor"];
                    if (proveedor is null) return BadRequest();

                    await _facturaService.CancelFactura(proveedor.Id, usrtype, id);
                    return NoContent();
                default:
                    var usr = (Usuario)HttpContext.Items["usuario"];
                    if (usr is null) return BadRequest();

                    await _facturaService.CancelFactura(usr.Id, usrtype, id);
                    return NoContent();
            }
        }

        /// <summary>
        /// Actualizar Estado de Factura.
        /// </summary>
        [HttpPut("actualizarestado/{id}")]
        public async Task<IActionResult> ActualizarEstadoFactura(int id, byte estado, string? comentario)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());

            string usrtype = (string)HttpContext.Items["usrtype"];
            if (string.IsNullOrEmpty(usrtype)) return BadRequest();

            switch (usrtype)
            {
                case "proveedor":
                    var proveedor = (Proveedor)HttpContext.Items["proveedor"];
                    if (proveedor is null) return BadRequest();

                    await _facturaService.ActualizarEstadoFactura(proveedor.Id, usrtype, id, estado, comentario);
                    return NoContent();
                default:
                    var usr = (Usuario)HttpContext.Items["usuario"];
                    if (usr is null) return BadRequest();

                    await _facturaService.ActualizarEstadoFactura(usr.Id, usrtype, id, estado, comentario);
                    return NoContent();
            }
        }

        /// <summary>
        /// Actualizar Factura.
        /// </summary>
        [HttpPut("actualizar/{id}")]
        public async Task<IActionResult> ActualizarFactura(int id, int proyecto, int aprobador)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());

            string usrtype = (string)HttpContext.Items["usrtype"];
            if (string.IsNullOrEmpty(usrtype)) return BadRequest();

            switch (usrtype)
            {
                case "proveedor":
                    var proveedor = (Proveedor)HttpContext.Items["proveedor"];
                    if (proveedor is null) return BadRequest();

                    await _facturaService.ActualizarFactura(proveedor.Id, usrtype, id, proyecto, aprobador);
                    return NoContent();
                default:
                    var usr = (Usuario)HttpContext.Items["usuario"];
                    if (usr is null) return BadRequest();

                    await _facturaService.ActualizarFactura(usr.Id, usrtype, id, proyecto, aprobador);
                    return NoContent();
            }
        }
    }
}
