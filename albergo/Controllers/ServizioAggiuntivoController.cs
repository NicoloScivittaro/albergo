using albergo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace W8.D3.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServizioAggiuntivoController : ControllerBase
    {
        private readonly AlbergoContext _context;

        public ServizioAggiuntivoController(AlbergoContext context)
        {
            _context = context;
        }

        // GET: api/ServizioAggiuntivo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServizioAggiuntivo>>> GetServiziAggiuntivi()
        {
            return await _context.ServiziAggiuntivi.ToListAsync();
        }

        // GET: api/ServizioAggiuntivo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ServizioAggiuntivo>> GetServizioAggiuntivo(int id)
        {
            var servizioAggiuntivo = await _context.ServiziAggiuntivi.FindAsync(id);

            if (servizioAggiuntivo == null)
            {
                return NotFound();
            }

            return servizioAggiuntivo;
        }

        // POST: api/ServizioAggiuntivo
        [HttpPost]
        public async Task<ActionResult<ServizioAggiuntivo>> PostServizioAggiuntivo(ServizioAggiuntivo servizioAggiuntivo)
        {
            _context.ServiziAggiuntivi.Add(servizioAggiuntivo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetServizioAggiuntivo), new { id = servizioAggiuntivo.Id }, servizioAggiuntivo);
        }

        // PUT: api/ServizioAggiuntivo/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutServizioAggiuntivo(int id, ServizioAggiuntivo servizioAggiuntivo)
        {
            if (id != servizioAggiuntivo.Id)
            {
                return BadRequest();
            }

            _context.Entry(servizioAggiuntivo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServizioAggiuntivoExists(id))
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

        // DELETE: api/ServizioAggiuntivo/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServizioAggiuntivo(int id)
        {
            var servizioAggiuntivo = await _context.ServiziAggiuntivi.FindAsync(id);
            if (servizioAggiuntivo == null)
            {
                return NotFound();
            }

            _context.ServiziAggiuntivi.Remove(servizioAggiuntivo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ServizioAggiuntivoExists(int id)
        {
            return _context.ServiziAggiuntivi.Any(e => e.Id == id);
        }
    }
}