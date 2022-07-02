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
    public class AlbumsController : ControllerBase
    {
        private readonly Context _context;

        public AlbumsController(Context context)
        {
            _context = context;
        }

        // GET: api/Albums
        /// <summary>
        /// 获取所有专辑
        /// </summary>
        /// <remarks>
        /// GET: api/Albums
        /// 
        /// 需要管理员及以上权限
        /// </remarks>
        /// <returns>所有专辑</returns>
        [HttpGet]
        [Authorize(Roles = "Root,Administrator")]
        public async Task<ActionResult<IEnumerable<Album>>> GetAlbum()
        {
          if (_context.Album == null)
          {
              return NotFound();
          }
            return await _context.Album.ToListAsync();
        }

        // GET: api/Albums/5
        /// <summary>
        /// 获取专辑信息
        /// </summary>
        /// <remarks>
        /// GET: api/Albums/5
        /// 
        /// 允许匿名访问
        /// </remarks>
        /// <param name="id">专辑ID</param>
        /// <returns>专辑信息</returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
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
        /// <summary>
        /// 修改指定ID的专辑信息
        /// </summary>
        /// <remarks>
        /// PUT: api/Albums/5
        /// 
        /// 需要管理员及以上权限
        /// 
        /// JSON形式传输
        /// </remarks>
        /// <param name="id">专辑ID</param>
        /// <param name="album">专辑对象</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Root,Administrator")]
        public async Task<IActionResult> PutAlbum(long id, Album album)
        {
            if (id != album.Id)
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
        /// <summary>
        /// 增加专辑
        /// </summary>
        /// <remarks>
        /// POST: api/Albums
        /// 
        /// 需要管理员及以上权限
        /// 
        /// JSON形式传输
        /// </remarks>
        /// <param name="album">专辑对象</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Root,Administrator")]
        public async Task<ActionResult<Album>> PostAlbum(Album album)
        {
          if (_context.Album == null)
          {
              return Problem("Entity set 'Context.Album'  is null.");
          }
            _context.Album.Add(album);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAlbum", new { id = album.Id }, album);
        }

        // DELETE: api/Albums/5
        /// <summary>
        /// 删除专辑
        /// </summary>
        /// <remarks>
        /// DELETE: api/Albums/5
        /// 
        /// 需要管理员及以上权限
        ///
        /// 若成功返回204
        /// </remarks>
        /// <param name="id">专辑ID</param>
        /// <returns>若成功返回204</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Root,Administrator")]
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
            return (_context.Album?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
