using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalProveedor.Authorization;
using PortalProveedor.Database;
using PortalProveedor.Entities;

namespace PortalProveedor.Controllers
{
    [Authorize(new string[] { "Proveedor", "Administrador/Contable" })]
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly PortalProveedorContext _context;
        private readonly ILogger<ClienteController> _logger;

        public ClienteController(PortalProveedorContext context, ILogger<ClienteController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Cliente
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cliente>>> GetCliente()
        {
            _logger.LogInformation("Obteniendo clientes...");

            return await _context.Clientes.ToListAsync();
        }

        [HttpGet("ByUsuario")]
        public async Task<ActionResult<IEnumerable<Cliente>>> GetClientesByUsuario()
        {
            var usr = (Usuario)HttpContext.Items["usuario"];
            if (usr is null) return BadRequest();

            var clientes = await _context.Clientes
                .Include(i => i.Sociedads)
                .ThenInclude(i => i.RolSociedadUsuarios).Where(w => w.Sociedads.Any(a => a.RolSociedadUsuarios.Any(a2 => a2.Usuario == usr.Id))).ToListAsync();
            return clientes;
        }

        // GET: api/Cliente/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cliente>> GetCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
            {
                return NotFound();
            }

            return cliente;
        }

        // PUT: api/Cliente/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCliente(int id, Cliente cliente)
        {
            if (id != cliente.Id)
            {
                return BadRequest();
            }

            _context.Entry(cliente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClienteExists(id))
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

        // POST: api/Cliente
        [HttpPost]
        public async Task<ActionResult<Cliente>> PostCliente(Cliente cliente)
        {
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCliente", new { id = cliente.Id }, cliente);
        }

        // DELETE: api/Cliente/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClienteExists(int id)
        {
            return _context.Clientes.Any(e => e.Id == id);
        }
    }
}
