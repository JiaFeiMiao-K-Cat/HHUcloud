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

    /// <summary>
    /// 获取所有歌曲列表
    /// </summary>
    /// <remarks>
    /// GET: api/Songs
    /// 
    /// 允许匿名访问
    /// </remarks>
    /// <returns>歌曲列表</returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<Song>>> GetSong()
    {
        if (_context.Song == null)
        {
            return NotFound();
        }

        var list = await _context.Song.ToListAsync();

        foreach (var song in list)
        {
            await FillSongInfoAsync(song);
        }

        return list;
    }

    /// <summary>
    /// 获取指定ID的歌曲信息
    /// </summary>
    /// <remarks>
    /// GET: api/Songs/5
    /// 
    /// 允许匿名访问
    /// </remarks>
    /// <param name="id">歌曲ID</param>
    /// <returns>歌曲信息(包含专辑信息和歌手信息)</returns>
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

        await FillSongInfoAsync(song);

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
            await FillSongInfoAsync(song);
        }

        return list;
    }

    /// <summary>
    /// 更新歌曲信息
    /// </summary>
    /// <remarks>
    /// PUT: api/Songs/5
    /// 
    /// 需要管理员及以上权限
    /// </remarks>
    /// <param name="id">歌曲ID</param>
    /// <param name="song">歌曲信息</param>
    /// <returns></returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Root,Administrator")]
    public async Task<IActionResult> PutSong(long id, Song song)
    {
        if (id != song.SongId)
        {
            return BadRequest();
        }

        if (!SongExists(id))
        {
            return NotFound();
        }

        _context.ArtistHasSong?
            .RemoveRange(_context.ArtistHasSong
                .Where(e => e.SongId == id));

        _context.AlbumHasSong?
            .RemoveRange(_context.AlbumHasSong
                .Where(e => e.SongId == id));
        var album = new AlbumHasSong()
        {
            AlbumId = song.AlbumId,
            SongId = song.SongId,
        };
        _context.AlbumHasSong?.Add(album);
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
        _context.Update(song);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetSong", new { id = song.SongId }, song);
    }

    /// <summary>
    /// 改变歌曲状态
    /// </summary>
    /// <remarks>
    /// POST: api/Songs/ChangeStatus/5
    /// 
    /// 管理员及以上权限
    /// </remarks>
    /// <param name="id">歌曲ID</param>
    /// <returns>歌曲信息</returns>
    [HttpPost("ChangeStatus/{id}")]
    [Authorize(Roles = "Root,Administrator")]
    public async Task<ActionResult<Song>> ChangeStatus(long id)
    {
        if (_context.Song == null)
        {
            return Problem("Entity set 'Context.Song'  is null.");
        }
        if (!SongExists(id))
        {
            return NotFound($"Song {id} is not found");
        }
        var song = _context.Song.First(e => e.SongId == id);

        song.Accessible = !song.Accessible;

        _context.Song.Update(song);

        await _context.SaveChangesAsync();

        return CreatedAtAction("GetSong", new { id = id }, song);
    }

    /// <summary>
    /// 添加歌曲
    /// </summary>
    /// <remarks>
    /// POST: api/Songs
    /// 
    /// 管理员及以上权限
    /// </remarks>
    /// <param name="song">歌曲信息</param>
    /// <returns>处理后的歌曲信息</returns>
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

    /// <summary>
    /// 搜索歌曲
    /// </summary>
    /// <remarks>
    /// POST: api/Songs/Search
    /// 
    /// 允许匿名访问
    /// 
    /// JSON形式传输
    /// </remarks>
    /// <param name="keywords">关键词</param>
    /// <returns>歌曲列表</returns>
    [HttpPost("Search")]
    [AllowAnonymous]
    public async Task<ActionResult<List<Song>>> Search(string keywords)
    {
        var terms = keywords.Split(' ');
        var list = _context.Song.ToList()
            .Where(q => terms.All(term => q.Title.Contains(term)))
            .ToList();
        foreach (var item in list)
        {
            await FillSongInfoAsync(item);
        }
        return Ok(list);
    }

    /// <summary>
    /// 删除歌曲
    /// </summary>
    /// <remarks>
    /// DELETE: api/Songs/5
    /// 
    /// 管理员及以上权限
    /// </remarks>
    /// <param name="id">歌曲ID</param>
    /// <returns></returns>
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

        _context.ArtistHasSong?
            .RemoveRange(_context.ArtistHasSong
                .Where(e => e.SongId == id));

        _context.AlbumHasSong?
            .RemoveRange(_context.AlbumHasSong
                .Where(e => e.SongId == id));

        _context.PlaylistHasSong?
            .RemoveRange(_context.PlaylistHasSong
                .Where(e => e.SongId == id));

        _context.PlayRecord?
            .RemoveRange(_context.PlayRecord
                .Where(e => e.SongId == id));

        await _context.SaveChangesAsync();

        return NoContent();
    }
    private async Task FillSongInfoAsync(Song song)
    {
        if (song == null)
        {
            return;
        }

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

    private bool SongExists(long id)
    {
        return (_context.Song?.Any(e => e.SongId == id)).GetValueOrDefault();
    }
}
