using Microsoft.AspNetCore.Mvc;
using PortalProveedor.Authorization;
using PortalProveedor.Database;
using PortalProveedor.Entities;
using PortalProveedor.Helpers;
using PortalProveedor.Models.FlujoAprobacionFacturas;
using PortalProveedor.Services;

namespace PortalProveedor.Controllers
{
    [Authorize(new string[] { "Proveedor", "Administrador/Contable" })]
    [ApiController]
    [Route("api/[controller]")]
    public class FlujoAprobacionFacturaController : ControllerBase
    {
        private readonly PortalProveedorContext _context;
        private readonly ILogger<FlujoAprobacionFacturaController> _logger;
        private readonly IFlujoAprobacionFacturaService _flujoAprobacionFacturaService;

        public FlujoAprobacionFacturaController(PortalProveedorContext context, ILogger<FlujoAprobacionFacturaController> logger, IFlujoAprobacionFacturaService flujoAprobacionFacturaService)
        {
            _context = context;
            _logger = logger;
            _flujoAprobacionFacturaService = flujoAprobacionFacturaService;
        }

        /// <summary>
        /// Obtener Lista de Flujos de Aprobación de Facturas.
        /// </summary>
        [HttpGet("simple")]
        public ActionResult<IEnumerable<FlujoAprobacionFacturaSimpleResponse>> GetFlujoAprobacionFacturaSimple([FromQuery] BusquedaFlujoAprobacionFacturaRequest request)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());

            var usr = (Usuario)HttpContext.Items["usuario"];
            if (usr is null) return BadRequest();

            IEnumerable<FlujoAprobacionFacturaSimpleResponse> response = _flujoAprobacionFacturaService.GetFlujoAprobacionFacturaSimple(usr.Id, usr.RolSociedadUsuarios.Select(s => s.Sociedad), request);
            return Ok(response);
        }
    }
}
