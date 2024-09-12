using Microsoft.AspNetCore.Mvc;
using PortalProveedor.Authorization;
using PortalProveedor.Database;
using PortalProveedor.Entities;
using PortalProveedor.Helpers;
using PortalProveedor.Models.Pedidos;
using PortalProveedor.Models.Proyectos;
using PortalProveedor.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PortalProveedor.Controllers
{
    [Authorize(new string[] { "Proveedor", "Administrador/Contable" })]
    [Route("api/[controller]")]
    [ApiController]
    public class PedidoController : ControllerBase
    {
        private readonly PortalProveedorContext _context;
        private readonly ILogger<PedidoController> _logger;
        private readonly IPedidoService _pedidoService;


        public PedidoController(PortalProveedorContext context, ILogger<PedidoController> logger, IPedidoService pedidoService)
        {
            _context = context;
            _logger = logger;
            _pedidoService = pedidoService;
        }

        /// <summary>
        /// Agregar nuevo Pedido.
        /// </summary>
        [HttpPost("alta-pedido")]
        public async Task<IActionResult> AltaPedidoAsync([FromForm]AltaPedidoRequest dto)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());

            string usrtype = (string)HttpContext.Items["usrtype"];
            if (string.IsNullOrEmpty(usrtype)) return BadRequest();

            switch (usrtype)
            {
                case "proveedor":
                    var proveedor = (Proveedor)HttpContext.Items["proveedor"];
                    if (proveedor is null) return BadRequest();

                    await _pedidoService.AltaPedido(proveedor.Id, usrtype, dto);
                    return Ok(new { message = "Alta pedido correctamente" });
                default:
                    var usr = (Usuario)HttpContext.Items["usuario"];
                    if (usr is null) return BadRequest();

                    await _pedidoService.AltaPedido(usr.Id, usrtype, dto);
                    return Ok(new { message = "Alta pedido correctamente" });
            }
        }

        /// <summary>
        /// Obtener Lista de Pedidos por Sociedad y/o Proveedor.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PedidoResponse>>> GetPedidos()
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());

            string usrtype = (string)HttpContext.Items["usrtype"];
            if (string.IsNullOrEmpty(usrtype)) return BadRequest();

            switch (usrtype)
            {
                case "proveedor":
                    var proveedorctx = (Proveedor)HttpContext.Items["proveedor"];
                    if (proveedorctx is null) return BadRequest();

                    IEnumerable<PedidoResponse> response = await _pedidoService.GetPedidos(proveedorctx.Id, proveedorctx.LoginProveedorSociedads.Select(s => s.Sociedad));
                    return response.ToList();
                default:
                    var usr = (Usuario)HttpContext.Items["usuario"];
                    if (usr is null) return BadRequest();

                    response = await _pedidoService.GetPedidos(usr.Id, usr.RolSociedadUsuarios.Select(s => s.Sociedad));
                    return response.ToList();
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PedidoResponse>> GetPedido(int id)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());

            string usrtype = (string)HttpContext.Items["usrtype"];
            if (string.IsNullOrEmpty(usrtype)) return BadRequest();

            switch (usrtype)
            {
                case "proveedor":
                    var proveedorctx = (Proveedor)HttpContext.Items["proveedor"];
                    if (proveedorctx is null) return BadRequest();

                    PedidoResponse response = await _pedidoService.GetPedido(proveedorctx.Id, usrtype, id);
                    return response;
                default:
                    var usr = (Usuario)HttpContext.Items["usuario"];
                    if (usr is null) return BadRequest();

                    response = await _pedidoService.GetPedido(usr.Id, usrtype, id);
                    return response;
            }
        }

        /// <summary>
        /// Obtener Lista de Pedidos por Sociedad y/o Proveedor.
        /// </summary>
        [HttpGet("bysociedadyproveedor")]
        public async Task<ActionResult<IEnumerable<PedidoResponse>>> GetPedidoBySociedadAndProveedorAsync(int? sociedad, int? proveedor, int? numero)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());

            string usrtype = (string)HttpContext.Items["usrtype"];
            if (string.IsNullOrEmpty(usrtype)) return BadRequest();

            switch (usrtype)
            {
                case "proveedor":
                    var proveedorctx = (Proveedor)HttpContext.Items["proveedor"];
                    if (proveedorctx is null) return BadRequest();

                    IEnumerable<PedidoResponse> response = await _pedidoService.GetPedidosBySociedadAndProveedor(proveedorctx.Id, usrtype, sociedad, proveedor, numero);
                    return response.ToList();
                default:
                    var usr = (Usuario)HttpContext.Items["usuario"];
                    if (usr is null) return BadRequest();

                    response = await _pedidoService.GetPedidosBySociedadAndProveedor(usr.Id, usrtype, sociedad, proveedor, numero);
                    return response.ToList();
            }
        }

        /// <summary>
        /// Actualizar Pedido.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPedido(int id, [FromForm] EditPedidoRequest dto)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());

            string usrtype = (string)HttpContext.Items["usrtype"];
            if (string.IsNullOrEmpty(usrtype)) return BadRequest();

            switch (usrtype)
            {
                case "proveedor":
                    var proveedorctx = (Proveedor)HttpContext.Items["proveedor"];
                    if (proveedorctx is null) return BadRequest();

                    await _pedidoService.ActualizarPedido(id, proveedorctx.Id, usrtype, dto);
                    return Ok(new { message = "Edición pedido correctamente" });
                default:
                    var usr = (Usuario)HttpContext.Items["usuario"];
                    if (usr is null) return BadRequest();

                    await _pedidoService.ActualizarPedido(id, usr.Id, usrtype, dto);
                    return Ok(new { message = "Edición pedido correctamente" });
            }
        }
    }
}
