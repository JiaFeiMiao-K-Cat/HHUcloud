using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EMO_Cloud.Data;
using EMO_Cloud.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace EMO_Cloud.Controllers
{
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
            if (_context.Song == null || !SongExists(id))
            {
                return NotFound();
            }

            var song = await _context.Song.FindAsync(id);

            if (song == null)
            {
                return NotFound();
            }

            long userId = GetUserId();

            if (_context.User.Any(e => e.Id == userId))
            {
                PlayRecord playRecord = await _context.PlayRecord.FirstOrDefaultAsync(e => e.UserId == userId);
                if (playRecord == null)
                {
                    playRecord = new PlayRecord();
                    playRecord.UserId = userId;
                    playRecord.Id = 0;
                    playRecord.SongId = id;
                    _context.PlayRecord.Add(playRecord);
                }
                playRecord.LastTime = DateTime.Now;
                playRecord.Count++;
            }

            song.Count++;

            await _context.SaveChangesAsync();

            return song;
        }

        /// <summary>
        /// 点击量最高的歌曲列表
        /// </summary>
        /// <remarks>
        /// GET: api/Songs/Top/5
        /// </remarks>
        /// <param name="size">列表长度</param>
        /// <returns>歌曲列表, 长度超过曲库大小时只返回曲库</returns>
        [HttpGet("Top/{size}")]
        [AllowAnonymous]
        public ActionResult<List<Song>> Top(int size)
        {
            if (_context.Song == null)
            {
                return NotFound();
            }
            
            List<Song> list = _context.Song.OrderBy(e => e.Count).Take(size).ToList();

            foreach (Song song in list)
            {
                song.ArtistId = _context.ArtistHasSongs?
                    .Where(e => e.SongId == song.Id)
                    .Select(p => p.ArtistId).ToList();
            }

            return list;
        }
        // GET: api/Songs/Artists/5
        /// <summary>
        /// 指定ID的歌曲的歌手列表
        /// </summary>
        /// <remarks>
        /// GET: api/Songs/Artist/5
        /// </remarks>
        /// <param name="songId">歌曲ID</param>
        /// <returns>歌手</returns>
        [HttpGet("Artist/{songId}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Tuple<string, long>>>> Artist(long songId)
        {
            if (_context.Artist == null || _context.Song == null || !SongExists(songId))
            {
                return NotFound();
            }

            return _context.ArtistHasSongs?
                .Where(e => e.SongId == songId)
                .Select(p => new Tuple<string, long>(
                    (_context.Artist.FirstOrDefault(q => q.Id == p.ArtistId)??new Artist()).Name, 
                    p.ArtistId))
                .ToList();
        }

        // POST: api/Songs/AddSong
        /// <summary>
        /// 添加歌曲
        /// </summary>
        /// <param name="song"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<Song>> PostSong(Song _song)
        {
            if (_context.Song == null)
            {
                return Problem("Entity set 'Context.Song'  is null.");
            }
            Song song = new Song();
            _context.Song.Add(song);
            try
            {
                foreach (long artist in song.ArtistId)
                {
                    ArtistHasSongs artistHasSongs = _context.ArtistHasSongs
                        .FirstOrDefault(e => e.ArtistId == artist && e.SongId == song.Id);
                    if (artistHasSongs == null)
                    {
                        artistHasSongs = new ArtistHasSongs();
                        artistHasSongs.Id = 0;
                        artistHasSongs.SongId = song.Id;
                        artistHasSongs.ArtistId = artist;
                        _context.ArtistHasSongs.Add(artistHasSongs);
                    }
                } // add reltionship between artists and song
            }
            catch (Exception ex) { }
            AlbumHasSongs albumHasSongs = _context.AlbumHasSongs
                .FirstOrDefault(e => e.SongId == song.Id && e.AlbumId == song.AlbumId);
            if (albumHasSongs == null)
            {
                albumHasSongs = new AlbumHasSongs();
                albumHasSongs.Id = 0;
                albumHasSongs.SongId = song.Id;
                albumHasSongs.AlbumId = song.AlbumId;
                _context.AlbumHasSongs.Add(albumHasSongs);
            } // add reltionship between album and song
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSong", new { id = song.Id }, song);
        }

        // POST: api/Songs/Search
        /// <summary>
        /// 搜索歌曲
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        [HttpPost("Search")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Song>>> Search([FromBody]string pattern)
        {
            var terms = pattern.Split(' ');
            var list = _context.Song.ToList()

                .Where(q => terms.All(term => q.Title.Contains(term)))
                .ToList();
            foreach (var item in list)
            {
                item.ArtistId = _context.ArtistHasSongs
                    .Where(e => e.SongId == item.Id)
                    .Select(p => p.ArtistId)
                    .ToList();
            }
            return Ok(list);
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
            _context.AlbumHasSongs.RemoveRange(_context.AlbumHasSongs.Where(e => e.SongId == id));
            _context.ArtistHasSongs.RemoveRange(_context.ArtistHasSongs.Where(e => e.SongId == id));
            _context.PlayListHasSongs.RemoveRange(_context.PlayListHasSongs.Where(e => e.SongId != id));
            _context.Song.Remove(song);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        // GET: api/Songs/Artists/5
        [HttpGet("Artists/{songId}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Tuple<string, long>>>> Artists(int songId)
        {
            if (_context.Song == null || !SongExists(songId))
            {
                return NotFound();
            }
            return _context.ArtistHasSongs?
                .Where(e => e.SongId == songId)
                .Select(p => new Tuple<string, long>(
                    (_context.Artist.FirstOrDefault(q => q.Id == p.ArtistId)??new Artist()).Name,
                    p.ArtistId))
                .ToList();
        }
        // GET: api/Songs/Albums/5
        [HttpGet("Albums/{songId}")]
        [AllowAnonymous]
        public async Task<ActionResult<Tuple<string, long>>> Albums(long songId)
        {
            if (_context.Song == null || !SongExists(songId))
            {
                return NotFound();
            }
            Song song = await _context.Song.FindAsync(songId);
            Album album = await _context.Album.FindAsync(song.AlbumId);

            return new Tuple<string, long>((album ?? new Album()).Name, song.AlbumId);
        }

        private bool SongExists(long id)
        {
            return (_context.Song?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        /// <summary>
        /// copy from: https://stackoverflow.com/questions/50580232/get-userid-from-jwt-on-all-controller-methods
        /// </summary>
        /// <returns>当前Token的用户ID</returns>
        protected long GetUserId()
        {
            return long.Parse(this.User.Claims.First(i => i.Type == "UserId").Value);
        }
    }
}
