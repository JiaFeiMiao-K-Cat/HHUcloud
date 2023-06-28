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
public class PlaylistsController : ControllerBase
{
    private readonly Context _context;

    public PlaylistsController(Context context)
    {
        _context = context;
    }

    // GET: api/Playlists
    [HttpGet]
    [Authorize(Roles = "Root,Administrator")]
    public async Task<ActionResult<IEnumerable<Playlist>>> GetPlaylist()
    {
      if (_context.Playlist == null)
      {
          return NotFound();
      }
        return await _context.Playlist.ToListAsync();
    }

    // GET: api/Playlists/5
    [HttpGet("{id}")]
    [Authorize(Roles = "Root,Administrator,User,Guest")]
    public async Task<ActionResult<Playlist>> GetPlaylist(long id)
    {
      if (_context.Playlist == null)
      {
          return NotFound();
      }
        var playlist = await _context.Playlist.FindAsync(id);

        if (playlist == null)
        {
            return NotFound();
        }

        

        return playlist;
    }

    // PUT: api/Playlists/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    [Authorize(Roles = "Root,Administrator,User")]
    public async Task<IActionResult> PutPlaylist(long id, Playlist playlist)
    {
        if (id != playlist.PlaylistId)
        {
            return BadRequest();
        }

        _context.Entry(playlist).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PlaylistExists(id))
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

    // POST: api/Playlists
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize(Roles = "Root,Administrator")]
    public async Task<ActionResult<Playlist>> PostPlaylist(Playlist playlist)
    {
      if (_context.Playlist == null)
      {
          return Problem("Entity set 'Context.Playlist'  is null.");
      }
        _context.Playlist.Add(playlist);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetPlaylist", new { id = playlist.PlaylistId }, playlist);
    }

    /// <summary>
    /// 创建歌单
    /// </summary>
    /// <remarks>
    /// POST: api/Playlists/Create
    /// 
    /// JSON形式传参
    /// </remarks>
    /// <param name="playlist">歌单信息(标题必选)</param>
    /// <returns>歌单信息</returns>
    [HttpPost("Create")]
    [Authorize(Roles = "Root,Administrator,User")]
    public async Task<ActionResult<Playlist>> CreatePlaylist(Playlist playlist)
    {
        if (_context.Playlist == null)
        {
            return Problem("Entity set 'Context.Playlist'  is null.");
        }

        playlist.PlaylistId = 0;
        playlist.UserId = GetUserId();
        playlist.Created = DateTime.UtcNow;

        _context.Playlist.Add(playlist);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetPlaylist", new { id = playlist.PlaylistId }, playlist);
    }

    /// <summary>
    /// 向歌单添加歌曲
    /// </summary>
    /// <remarks>
    /// POST: api/PlayLists/AddSong
    /// 
    /// JSON形式传输
    /// </remarks>
    /// <param name="item">相关信息, 歌单和歌曲ID必选</param>
    /// <returns></returns>
    [HttpPost("AddSong")]
    public async Task<ActionResult> AddSong(PlaylistHasSong item)
    {
        if (item == null)
        {
            return BadRequest("null");
        }
        var playlistId = item.PlaylistId;
        var songId = item.SongId;
        if (_context.Playlist == null)
        {
            return Problem("Entity set 'Context.PlayList'  is null.");
        }
        if (!PlaylistExists(playlistId))
        {
            return NotFound();
        }

        var userId = GetUserId();
        var user = _context.User.FirstOrDefault(e => e.UserId == userId);
        if (user == null
            || (userId != _context.Playlist.Find(playlistId).UserId
                && user.UserRole <= Role.User))
        {
            return Problem("Permission denied");
        }

        if (_context.PlaylistHasSong.Any(e => e.PlaylistId == playlistId && e.SongId == songId))
        {
            return Ok("Already exists.");
        }
        var playListHasSongs = new PlaylistHasSong() 
        {
            SongId = songId,
            PlaylistId = playlistId,
            Added = DateTime.UtcNow
        };
        _context.PlaylistHasSong.Add(playListHasSongs);
        await _context.SaveChangesAsync();

        return Ok();
    }

    /// <summary>
    /// 从歌单删除歌曲
    /// </summary>
    /// <remarks>
    /// POST: api/Playlists/RemoveSong
    /// 
    /// JSON形式传输
    /// </remarks>
    /// <param name="item">相关信息, 歌单和歌曲ID必选</param>
    /// <returns></returns>
    [HttpPost("RemoveSong")]
    public async Task<ActionResult> RemoveSong(PlaylistHasSong item)
    {
        if (item == null)
        {
            return BadRequest("null");
        }
        var playlistId = item.PlaylistId;
        var songId = item.SongId;
        Console.WriteLine($"{playlistId}, {songId}");
        if (_context.Playlist == null)
        {
            return Problem("Entity set 'Context.PlayList'  is null.");
        }
        if (!PlaylistExists(playlistId))
        {
            return NotFound();
        }

        var userId = GetUserId();
        var user = _context.User.FirstOrDefault(e => e.UserId == userId);
        if (user == null 
            || (userId != _context.Playlist.Find(playlistId).UserId 
                && user.UserRole <= Role.User))
        {
            return Problem("Permission denied");
        }

        var playListHasSongs = _context.PlaylistHasSong?
            .FirstOrDefault(e => e.PlaylistId == playlistId && e.SongId == songId);
        if (playListHasSongs != null)
        {
            _context.PlaylistHasSong.Remove(playListHasSongs);
            await _context.SaveChangesAsync();
        }

        return Ok();
    }

    // DELETE: api/Playlists/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePlaylist(long id)
    {
        if (_context.Playlist == null)
        {
            return NotFound();
        }
        var playlist = await _context.Playlist.FindAsync(id);
        if (playlist == null)
        {
            return NotFound();
        }

        _context.Playlist.Remove(playlist);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool PlaylistExists(long id) => (_context.Playlist?.Any(e => e.PlaylistId == id)).GetValueOrDefault();

    /// <summary>
    /// copy from: https://stackoverflow.com/questions/50580232/get-userid-from-jwt-on-all-controller-methods
    /// </summary>
    /// <returns>当前Token的用户ID</returns>
    protected long GetUserId()
    {
        var userId = this.User.Claims.FirstOrDefault(i => i.Type == "UserId");
        if (userId == null)
        {
            return -1;
        } // Without Token
        return long.Parse(userId.Value);
    }
}
