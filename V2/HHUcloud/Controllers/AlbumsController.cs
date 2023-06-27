using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HHUcloud.Data;
using HHUcloud.Model;

namespace HHUcloud.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AlbumsController : ControllerBase
{
    private readonly Context _context;

    public AlbumsController(Context context)
    {
        _context = context;
    }

    // GET: api/Albums
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Album>>> GetAlbum()
    {
      if (_context.Album == null)
      {
          return NotFound();
      }
        return await _context.Album.ToListAsync();
    }

    // GET: api/Albums/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Album>> GetAlbum(long id)
    {
      if (_context.Album == null)
      {
          return NotFound();
      }
        var album = await _context.Album.FindAsync(id);

        if (album == null)
        {
            return NotFound();
        }

        return album;
    }

    // PUT: api/Albums/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAlbum(long id, Album album)
    {
        if (id != album.AlbumId)
        {
            return BadRequest();
        }

        _context.Entry(album).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!AlbumExists(id))
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

    // POST: api/Albums
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Album>> PostAlbum(Album album)
    {
        if (_context.Album == null)
        {
            return Problem("Entity set 'Context.Album'  is null.");
        }
        if (album.ArtistIds != null)
        {
            foreach(var artistId in album.ArtistIds)
            {
                await _context.ArtistHasAlbums.AddAsync(new ArtistHasAlbum()
                {
                    AlbumId = album.AlbumId,
                    ArtistId = artistId
                });
            }
        }
        _context.Album.Add(album);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetAlbum", new { id = album.AlbumId }, album);
    }

    // DELETE: api/Albums/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAlbum(long id)
    {
        if (_context.Album == null)
        {
            return NotFound();
        }
        var album = await _context.Album.FindAsync(id);
        if (album == null)
        {
            return NotFound();
        }

        _context.Album.Remove(album);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool AlbumExists(long id)
    {
        return (_context.Album?.Any(e => e.AlbumId == id)).GetValueOrDefault();
    }
}
