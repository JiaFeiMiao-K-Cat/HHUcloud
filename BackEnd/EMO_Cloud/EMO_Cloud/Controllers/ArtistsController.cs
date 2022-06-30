using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EMO_Cloud.Data;
using EMO_Cloud.Models;

namespace EMO_Cloud.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtistsController : ControllerBase
    {
        private readonly Context _context;

        public ArtistsController(Context context)
        {
            _context = context;
        }

        // GET: api/Artists
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Artist>>> GetArtist()
        {
          if (_context.Artist == null)
          {
              return NotFound();
          }
            return await _context.Artist.ToListAsync();
        }

        // GET: api/Artists/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Artist>> GetArtist(long id)
        {
          if (_context.Artist == null)
          {
              return NotFound();
          }
            var artist = await _context.Artist.FindAsync(id);

            if (artist == null)
            {
                return NotFound();
            }

            return artist;
        }

        // PUT: api/Artists/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutArtist(long id, Artist artist)
        {
            if (id != artist.Id)
            {
                return BadRequest();
            }

            _context.Entry(artist).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArtistExists(id))
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

        // POST: api/Artists
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Artist>> PostArtist(Artist artist)
        {
          if (_context.Artist == null)
          {
              return Problem("Entity set 'Context.Artist'  is null.");
          }
            _context.Artist.Add(artist);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetArtist", new { id = artist.Id }, artist);
        }

        [HttpGet("Songs/{artistId}")]
        public async Task<ActionResult<List<Tuple<string, long>>>> Songs(long artistId)
        {
            if (_context.Artist == null)
            {
                return Problem("Entity set 'Context.Artist'  is null.");
            }
            if (!_context.Artist.Any(e => e.Id == artistId))
            {
                return NotFound();
            }
            
            return _context.ArtistHasSongs?
                .Where(e => e.ArtistId == artistId)
                .Select(p => new Tuple<string, long>(
                    (_context.Song.FirstOrDefault(q => q.Id == p.SongId) ?? new Song()).Title,
                    p.SongId))
                .ToList();
        }

        // DELETE: api/Artists/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArtist(long id)
        {
            if (_context.Artist == null)
            {
                return NotFound();
            }
            var artist = await _context.Artist.FindAsync(id);
            if (artist == null)
            {
                return NotFound();
            }

            _context.Artist.Remove(artist);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ArtistExists(long id)
        {
            return (_context.Artist?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
