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
    public class AlbaranController : ControllerBase
    {
        private readonly PortalProveedorContext _context;
        private readonly ILogger<AlbaranController> _logger;

        public AlbaranController(PortalProveedorContext context, ILogger<AlbaranController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Albaran
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Albaran>>> GetAlbaran()
        {
            return await _context.Albarans.ToListAsync();
        }

        // GET: api/Albaran/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Albaran>> GetAlbaran(int id)
        {
            var albaran = await _context.Albarans.FindAsync(id);

            if (albaran == null)
            {
                return NotFound();
            }

            return albaran;
        }

        // PUT: api/Albaran/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAlbaran(int id, Albaran albaran)
        {
            if (id != albaran.Id)
            {
                return BadRequest();
            }

            _context.Entry(albaran).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AlbaranExists(id))
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

        // POST: api/Albaran
        [HttpPost]
        public async Task<ActionResult<Albaran>> PostAlbaran(Albaran albaran)
        {
            _context.Albarans.Add(albaran);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAlbaran", new { id = albaran.Id }, albaran);
        }

        // DELETE: api/Albaran/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAlbaran(int id)
        {
            var albaran = await _context.Albarans.FindAsync(id);
            if (albaran == null)
            {
                return NotFound();
            }

            _context.Albarans.Remove(albaran);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AlbaranExists(int id)
        {
            return _context.Albarans.Any(e => e.Id == id);
        }
    }
}
