using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HHUcloud.Data;
using HHUcloud.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace HHUcloud.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ArtistsController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly Context _context;

    public ArtistsController(IConfiguration config, Context context)
    {
        _configuration = config;
        _context = context;
    }

    // GET: api/Artists
    [HttpGet]
    [Authorize(Roles = "Root,Administrator")]
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
    [AllowAnonymous]
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
    [Authorize(Roles = "Root,Administrator")]
    public async Task<IActionResult> PutArtist(long id, Artist artist)
    {
        if (id != artist.ArtistId)
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
    [Authorize(Roles = "Root,Administrator")]
    public async Task<ActionResult<Artist>> PostArtist(Artist artist)
    {
      if (_context.Artist == null)
      {
          return Problem("Entity set 'Context.Artist'  is null.");
      }
        _context.Artist.Add(artist);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetArtist", new { id = artist.ArtistId }, artist);
    }

    // DELETE: api/Artists/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Root,Administrator")]
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
        return (_context.Artist?.Any(e => e.ArtistId == id)).GetValueOrDefault();
    }
}
