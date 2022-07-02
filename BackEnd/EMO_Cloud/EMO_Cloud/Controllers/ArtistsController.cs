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
    public class ArtistsController : ControllerBase
    {
        private readonly Context _context;

        public ArtistsController(Context context)
        {
            _context = context;
        }

        // GET: api/Artists
        /// <summary>
        /// 获取所有歌手列表
        /// </summary>
        /// <remarks>
        /// GET: api/Artists
        /// 
        /// 需要管理员及以上权限
        /// </remarks>
        /// <returns>所有歌手列表</returns>
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
        /// <summary>
        /// 获取指定ID的歌手信息
        /// </summary>
        /// <remarks>
        /// GET: api/Artists/5
        /// 
        /// 允许匿名访问
        /// </remarks>
        /// <param name="id">歌手ID</param>
        /// <returns>歌手信息</returns>
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
        /// <summary>
        /// 修改指定ID的歌手信息
        /// </summary>
        /// <remarks>
        /// PUT: api/Artists/5
        /// 
        /// 需要管理员及以上权限
        /// 
        /// JSON形式传输
        /// </remarks>
        /// <param name="id">歌手ID</param>
        /// <param name="artist">歌手对象</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Root,Administrator")]
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
        /// <summary>
        /// 添加歌手
        /// </summary>
        /// <remarks>
        /// POST: api/Artists
        /// 
        /// 需要管理员及以上权限
        /// 
        /// JSON形式传输
        /// 
        /// 返回歌手信息
        /// </remarks>
        /// <param name="artist">歌手对象</param>
        /// <returns>歌手信息</returns>
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

            return CreatedAtAction("GetArtist", new { id = artist.Id }, artist);
        }
        /// <summary>
        /// 获取歌手所演唱的歌曲列表
        /// </summary>
        /// POST: api/Artists/Songs
        /// 
        /// 允许匿名访问
        /// 
        /// FormData形式传输
        /// 
        /// 返回含有歌曲名称(Item1)和歌曲ID(Item2)的元组
        /// <param name="artistId">歌手ID</param>
        /// <returns>返回含有歌曲名称(Item1)和歌曲ID(Item2)的元组</returns>
        [HttpGet("Songs/{artistId}")]
        [AllowAnonymous]
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
        /// <summary>
        /// 删除歌手
        /// </summary>
        /// <remarks>
        /// DELETE: api/Artists/5
        /// 
        /// 需要管理员及以上权限
        /// 
        /// 若成功返回204
        /// </remarks>
        /// <param name="id">歌手ID</param>
        /// <returns>若成功返回204</returns>
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
            return (_context.Artist?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
