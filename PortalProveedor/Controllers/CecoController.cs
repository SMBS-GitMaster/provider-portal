using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalProveedor.Authorization;
using PortalProveedor.Database;
using PortalProveedor.Entities;

namespace PortalProveedor.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CecoController : ControllerBase
    {
        private readonly PortalProveedorContext _context;
        private readonly ILogger<CecoController> _logger;

        public CecoController(PortalProveedorContext context, ILogger<CecoController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Ceco
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ceco>>> GetCeco()
        {
            return await _context.Cecos.ToListAsync();
        }

        // GET: api/Ceco/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Ceco>> GetCeco(int id)
        {
            var ceco = await _context.Cecos.FindAsync(id);

            if (ceco == null)
            {
                return NotFound();
            }

            return ceco;
        }

        // PUT: api/Ceco/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCeco(int id, Ceco ceco)
        {
            if (id != ceco.Id)
            {
                return BadRequest();
            }

            _context.Entry(ceco).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CecoExists(id))
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

        // POST: api/Ceco
        [HttpPost]
        public async Task<ActionResult<Ceco>> PostCeco(Ceco ceco)
        {
            _context.Cecos.Add(ceco);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCeco", new { id = ceco.Id }, ceco);
        }

        // DELETE: api/Ceco/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCeco(int id)
        {
            var ceco = await _context.Cecos.FindAsync(id);
            if (ceco == null)
            {
                return NotFound();
            }

            _context.Cecos.Remove(ceco);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CecoExists(int id)
        {
            return _context.Cecos.Any(e => e.Id == id);
        }
    }
}
