using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalProveedor.Authorization;
using PortalProveedor.Database;
using PortalProveedor.Entities;
using PortalProveedor.Helpers;
using PortalProveedor.Models.Sociedades;
using PortalProveedor.Models.Usuarios;
using PortalProveedor.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PortalProveedor.Controllers
{
    [Authorize(new string[] { "Proveedor", "Administrador/Contable" })]
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly PortalProveedorContext _context;
        private readonly ILogger<UsuarioController> _logger;
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(PortalProveedorContext context, ILogger<UsuarioController> logger, IUsuarioService usuarioService)
        {
            _context = context;
            _logger = logger;
            _usuarioService = usuarioService;
        }

        /// <summary>
        /// Obtener Usuario.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioResponse>>> GetUsuario()
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());

            var usr = (Usuario)HttpContext.Items["usuario"];
            if (usr is null) return BadRequest();

            UsuarioResponse response = _usuarioService.GetUsuario(usr.Id);
            return Ok(response);
        }

        [HttpGet("GetEstados")]
        public async Task<ActionResult<IEnumerable<EstadoUsuarioResponse>>> GetEstados()
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());

            var usr = (Usuario)HttpContext.Items["usuario"];
            if (usr is null) return BadRequest();

            var response = _usuarioService.GetEstados();
            return Ok(response);
        }

        [HttpGet("/nombre/{nombre}")]
        public async Task<ActionResult<UsuarioResponse>> GetUsuarioByNombre(string nombre)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());

            var usr = (Usuario)HttpContext.Items["usuario"];
            if (usr is null) return BadRequest();

            UsuarioResponse response = _usuarioService.GetUsuarioByNombre(usr.Id, nombre);
            return Ok(response);
        }

        [HttpGet("/email/{email}")]
        public async Task<ActionResult<UsuarioResponse>> GetUsuarioByEmail(string email)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());

            var usr = (Usuario)HttpContext.Items["usuario"];
            if (usr is null) return BadRequest();

            UsuarioResponse response = _usuarioService.GetUsuarioByEmail(usr.Id, email);
            return Ok(response);
        }

        [HttpGet("GetResponsablesActuales")]
        public async Task<ActionResult<IEnumerable<ListaUsuarioResponse>>> GetResponsablesActuales()
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());

            var usr = (Usuario)HttpContext.Items["usuario"];
            if (usr is null) return BadRequest();

            IEnumerable<ListaUsuarioResponse> response = _usuarioService.GetResponsablesActuales(usr.Id);
            return Ok(response);
        }

        // GET: api/Usuario/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            var rolSociedadUsuario = _context.RolSociedadUsuarios.Where(w => w.Usuario == id);
            if (usuario == null)
            {
                return NotFound();
            }

            usuario.RolSociedadUsuarios = rolSociedadUsuario.ToList();

            return usuario;
        }

        /// <summary>
        /// Obtener Lista de Usuarios por Sociedad.
        /// </summary>
        [HttpGet("byusuario")]
        public ActionResult<IEnumerable<ListaUsuarioResponse>> GetUsuariosByUsuario()
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());

            var usr = (Usuario)HttpContext.Items["usuario"];
            if (usr is null) return BadRequest();

            IEnumerable<ListaUsuarioResponse> response = _usuarioService.GetUsuariosByUsuario(usr.Id, usr.RolSociedadUsuarios.Select(s => s.Sociedad));
            return Ok(response);
        }

        /// <summary>
        /// Obtener Lista de Usuarios por Sociedad.
        /// </summary>
        [HttpGet("bysociedad/{sociedad}")]
        public ActionResult<IEnumerable<ListaUsuarioResponse>> GetUsuariosBySociedad(int sociedad)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());

            var usr = (Usuario)HttpContext.Items["usuario"];
            if (usr is null) return BadRequest();

            IEnumerable<ListaUsuarioResponse> response = _usuarioService.GetUsuariosBySociedad(usr.Id, usr.RolSociedadUsuarios.Select(s => s.Sociedad), sociedad);
            return Ok(response);
        }

        /// <summary>
        /// Obtener Lista de Usuarios por Cliente.
        /// </summary>
        [HttpGet("bycliente/{cliente}")]
        public ActionResult<IEnumerable<ListaUsuarioResponse>> GetUsuariosByCliente(int cliente)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());

            var usr = (Usuario)HttpContext.Items["usuario"];
            if (usr is null) return BadRequest();

            IEnumerable<ListaUsuarioResponse> response = _usuarioService.GetUsuariosByCliente(usr.Id, usr.RolSociedadUsuarios.Select(s => s.Sociedad), cliente);
            return Ok(response);
        }

        /// <summary>
        /// Lista todos los valores con sus ids de la tabla EstadoUsuario.
        /// </summary>
        [HttpGet("estados")]
        public ActionResult<IEnumerable<ListaEstadoUsuarioResponse>> GetUsuariosEstados()
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());

            var usr = (Usuario)HttpContext.Items["usuario"];
            if (usr is null) return BadRequest();

            IEnumerable<ListaEstadoUsuarioResponse> response = _usuarioService.GetUsuariosEstados(usr.Id, usr.RolSociedadUsuarios.Select(s => s.Sociedad));
            return Ok(response);
        }

        /// <summary>
        /// Lista con todos los valores con sus ids de la tabla Rol.
        /// </summary>
        [HttpGet("roles")]
        public ActionResult<IEnumerable<ListaRolResponse>> GetRoles()
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());

            var usr = (Usuario)HttpContext.Items["usuario"];
            if (usr is null) return BadRequest();

            IEnumerable<ListaRolResponse> response = _usuarioService.GetRoles(usr.Id, usr.RolSociedadUsuarios.Select(s => s.Sociedad));
            return Ok(response);
        }

        /// <summary>
        /// Actualizar Usuario.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, ModifUsuarioRequest dto)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());

            var usr = (Usuario)HttpContext.Items["usuario"];
            if (usr is null) return BadRequest();

            await _usuarioService.ActualizarUsuario(id, usr.Id, usr.Cliente, dto);
            return NoContent();
        }

        /// <summary>
        /// Agregar nuevo Usuario.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Usuario>> AltaUsuario(AltaUsuarioRequest dto)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());

            var usr = (Usuario)HttpContext.Items["usuario"];
            if (usr is null) return BadRequest();

            await _usuarioService.AltaUsuario(usr.Id, usr.Cliente, dto);
            return Ok(new { message = "Alta realizada correctamente" });
        }

        // DELETE: api/Usuario/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            var rolSociedadUsuario = _context.RolSociedadUsuarios.Where(s => s.Usuario == id);
            _context.RolSociedadUsuarios.RemoveRange(rolSociedadUsuario);
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }
}
