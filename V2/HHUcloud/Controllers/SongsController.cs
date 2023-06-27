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
public class SongsController : ControllerBase
{
    private readonly Context _context;

    public SongsController(Context context)
    {
        _context = context;
    }

    // GET: api/Songs
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<Song>>> GetSong()
    {
      if (_context.Song == null)
      {
          return NotFound();
      }
        return await _context.Song.ToListAsync();
    }

    // GET: api/Songs/5
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<Song>> GetSong(long id)
    {
      if (_context.Song == null)
      {
          return NotFound();
      }
        var song = await _context.Song.FindAsync(id);

        if (song == null)
        {
            return NotFound();
        }

        song.CoverLink = (await _context.Album
            .FirstOrDefaultAsync(e => e.AlbumId == song.AlbumId))?
            .CoverLink;

        return song;
    }
    
    /// <summary>
     /// 点击量最高的歌曲列表
     /// </summary>
     /// <remarks>
     /// GET: api/Songs/Top/5
     /// 
     /// 允许匿名访问
     /// </remarks>
     /// <param name="size">列表长度</param>
     /// <returns>歌曲列表, 长度超过曲库大小时只返回曲库</returns>
    [HttpGet("Top/{size}")]
    [AllowAnonymous]
    public async Task<ActionResult<List<Song>>> TopAsync(int size)
    {
        if (_context.Song == null)
        {
            return NotFound();
        }

        List<Song> list = _context.Song.OrderByDescending(e => e.Count).Take(size).ToList();

        foreach (Song song in list)
        {
            song.CoverLink = (await _context.Album?
                .FirstOrDefaultAsync(e => e.AlbumId == song.AlbumId))?
                .CoverLink;

            song.AlbumTitle = (await _context.Album?
                .FirstOrDefaultAsync(e => e.AlbumId == song.AlbumId))?
                .Title;

            song.ArtistIds = _context.ArtistHasSong?
                .Where(e => e.SongId == song.SongId)
                .Select(e => e.ArtistId)
                .ToList();

            if (song.ArtistIds != null)
            {
                song.ArtistNames = new List<string>();
                foreach (var artistId in song.ArtistIds)
                {
                    song.ArtistNames.Add(
                        _context.Artist?
                            .FirstOrDefault(e => e.ArtistId == artistId)?
                            .Name
                        );
                }
            }
        }

        return list;
    }

    // PUT: api/Songs/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    [Authorize(Roles = "Root,Administrator")]
    public async Task<IActionResult> PutSong(long id, Song song)
    {
        if (id != song.SongId)
        {
            return BadRequest();
        }

        _context.Entry(song).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!SongExists(id))
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

    // POST: api/Songs
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize(Roles = "Root,Administrator")]
    public async Task<ActionResult<Song>> PostSong(Song song)
    {
        if (_context.Song == null)
        {
            return Problem("Entity set 'Context.Song'  is null.");
        }
        if (SongExists(song.SongId))
        {
            return Problem($"Song {song.SongId} has already exists.");
        }
        _context.Song.Add(song);
        var album = new AlbumHasSong()
        {
            AlbumId = song.AlbumId,
            SongId = song.SongId,
        };
        _context.AlbumHasSong.Add(album);
        if (song.ArtistIds != null)
        {
            foreach (var artistId in song.ArtistIds)
            {
                await _context.ArtistHasSong.AddAsync(new ArtistHasSong()
                {
                    ArtistId = artistId,
                    SongId = song.SongId,
                });
            }
        }
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetSong", new { id = song.SongId }, song);
    }

    // DELETE: api/Songs/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Root,Administrator")]
    public async Task<IActionResult> DeleteSong(long id)
    {
        if (_context.Song == null)
        {
            return NotFound();
        }
        var song = await _context.Song.FindAsync(id);
        if (song == null)
        {
            return NotFound();
        }

        _context.Song.Remove(song);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool SongExists(long id)
    {
        return (_context.Song?.Any(e => e.SongId == id)).GetValueOrDefault();
    }
}
