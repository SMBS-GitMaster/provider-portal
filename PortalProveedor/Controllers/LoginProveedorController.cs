using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalProveedor.Authorization;
using PortalProveedor.Database;
using PortalProveedor.Entities;
using PortalProveedor.Helpers;
using PortalProveedor.Models.Usuarios;
using PortalProveedor.Services;

namespace PortalProveedor.Controllers
{
    [Authorize("Proveedor")]
    [Route("api/[controller]")]
    [ApiController]
    public class LoginProveedorController : ControllerBase
    {
        private ILoginProveedorService _loginProveedorService;
        private readonly PortalProveedorContext _context;
        private readonly ILogger<LoginProveedorController> _logger;

        public LoginProveedorController(PortalProveedorContext context, ILoginProveedorService loginProveedorService, ILogger<LoginProveedorController> logger)
        {
            _context = context;
            _loginProveedorService = loginProveedorService;
            _logger = logger;
        }

        /// <summary>
        /// Login de Proveedor.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate(AuthenticateRequest model)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.Errors());

            var response = _loginProveedorService.Authenticate(model);
            return Ok(response);
        }

        /// <summary>
        /// Obtener Lista de Login Proveedor.
        /// </summary>
        // GET: api/LoginProveedor
        [HttpGet]
        public async Task<ActionResult> GetLoginProveedor()
        {
            return Ok(await _loginProveedorService.GetLoginProveedor());
        }

        /// <summary>
        /// Obtener Login Proveedor por ID.
        /// </summary>
        // GET: api/LoginProveedor/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LoginProveedor>> GetLoginProveedor(short id)
        {
            var loginProveedor = await _context.LoginProveedors.FindAsync(id);
            if (loginProveedor == null) return NotFound();

            return loginProveedor;
        }

        /// <summary>
        /// Actualizar Login Proveedor.
        /// </summary>
        // PUT: api/LoginProveedor/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLoginProveedor(short id, LoginProveedor loginProveedor)
        {
            if (id != loginProveedor.Id) return BadRequest();

            _context.Entry(loginProveedor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoginProveedorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// 
        /// </summary>
        // POST: api/LoginProveedor
        [HttpPost]
        public async Task<ActionResult<LoginProveedor>> PostLoginProveedor(LoginProveedor loginProveedor)
        {
            _context.LoginProveedors.Add(loginProveedor);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (LoginProveedorExists(loginProveedor.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetLoginProveedor", new { id = loginProveedor.Id }, loginProveedor);
        }

        /// <summary>
        /// Eliminar Login Proveedor.
        /// </summary>
        // DELETE: api/LoginProveedor/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLoginProveedor(short id)
        {
            var loginProveedor = await _context.LoginProveedors.FindAsync(id);
            if (loginProveedor == null)
            {
                return NotFound();
            }

            _context.LoginProveedors.Remove(loginProveedor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LoginProveedorExists(int id)
        {
            return _context.LoginProveedors.Any(e => e.Id == id);
        }
    }
}
