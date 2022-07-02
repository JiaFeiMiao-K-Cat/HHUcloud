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
    public class PlayListsController : ControllerBase
    {
        private readonly Context _context;

        public PlayListsController(Context context)
        {
            _context = context;
        }

        // GET: api/PlayLists
        /// <summary>
        /// 获取所有歌曲列表
        /// </summary>
        /// <remarks>
        /// GET: api/PlayLists
        /// 
        /// 需要管理员及以上权限
        /// </remarks>
        /// <returns>所有歌曲列表</returns>
        [HttpGet]
        [Authorize(Roles = "Root,Administrator")]
        public async Task<ActionResult<IEnumerable<PlayList>>> GetPlayList()
        {
          if (_context.PlayList == null)
          {
              return NotFound();
          }
            return await _context.PlayList.ToListAsync();
        }

        // GET: api/PlayLists/5
        /// <summary>
        /// 获取指定ID的播放列表
        /// </summary>
        /// <remarks>
        /// GET: api/PlayLists/5
        /// 
        /// 允许匿名访问
        /// </remarks>
        /// <param name="id">歌曲列表ID</param>
        /// <returns>歌曲列表</returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<PlayList>> GetPlayList(long id)
        {
          if (_context.PlayList == null)
          {
              return NotFound();
          }
            var playList = await _context.PlayList.FindAsync(id);

            if (playList == null)
            {
                return NotFound();
            }

            playList.SongList = _context.PlayListHasSongs
                .Where(e => e.PlaylistId == id)
                .Select(p => p.SongId)
                .ToList();

            return playList;
        }

        // POST: api/PlayLists/AddSong
        /// <summary>
        /// 向歌单添加歌曲
        /// </summary>
        /// <remarks>
        /// POST: api/PlayLists/AddSong
        /// 
        /// FormData形式传输
        /// </remarks>
        /// <param name="playlistId">歌单ID</param>
        /// <param name="songId">歌曲ID</param>
        /// <returns></returns>
        [HttpPost("AddSong")]
        public async Task<ActionResult> AddSong([FromForm] long playlistId, [FromForm]long songId)
        {
            Console.WriteLine($"playlistId:{playlistId}, songId:{songId}");
            if (_context.PlayList == null)
            {
                return Problem("Entity set 'Context.PlayList'  is null.");
            }
            if (!PlayListExists(playlistId))
            {
                return NotFound();
            }
            if (GetUserId() != _context.PlayList.Find(playlistId).UserId)
            {
                return Problem("Not Correct User");
            }
            if (_context.PlayListHasSongs.Any(e => e.PlaylistId == playlistId && e.SongId == songId))
            {
                return Ok("Already exists.");
            }
            PlayListHasSongs playListHasSongs = new PlayListHasSongs();
            playListHasSongs.Id = 0;
            playListHasSongs.SongId = songId;
            playListHasSongs.PlaylistId = playlistId;
            _context.PlayListHasSongs.Add(playListHasSongs);
            await _context.SaveChangesAsync();

            return Ok();
        }
        // POST: api/PlayLists/RemoveSong
        /// <summary>
        /// 从歌单删除歌曲
        /// </summary>
        /// <remarks>
        /// POST: api/PlayLists/RemoveSong
        /// 
        /// FormData形式传输
        /// </remarks>
        /// <param name="playlistId">歌单ID</param>
        /// <param name="songId">歌曲ID</param>
        /// <returns></returns>
        [HttpPost("RemoveSong")]
        public async Task<ActionResult> RemoveSong([FromForm] long playlistId, [FromForm] long songId)
        {
            if (_context.PlayList == null)
            {
                return Problem("Entity set 'Context.PlayList'  is null.");
            }
            if (!PlayListExists(playlistId))
            {
                return NotFound();
            }
            if (GetUserId() != _context.PlayList.Find(playlistId).UserId)
            {
                return Problem("Not Correct User");
            }
            
            PlayListHasSongs playListHasSongs = _context.PlayListHasSongs
                .FirstOrDefault(e => e.PlaylistId == playlistId && e.SongId == songId);
            if (playListHasSongs != null)
            {
                _context.PlayListHasSongs.Remove(playListHasSongs);
                await _context.SaveChangesAsync();
            }

            return Ok();
        }
        // POST: api/PlayLists
        /// <summary>
        /// 增加歌单
        /// </summary>
        /// <remarks>
        /// POST: api/PlayLists
        /// 
        /// 需要管理员及以上权限
        /// 
        /// JSON形式传输
        /// </remarks>
        /// <param name="playList">歌单</param>
        /// <returns>歌单信息</returns>
        [HttpPost]
        [Authorize(Roles = "Root,Administrator")]
        public async Task<ActionResult<PlayList>> PostPlayList(PlayList playList)
        {
            if (_context.PlayList == null)
            {
                return Problem("Entity set 'Context.PlayList'  is null.");
            }
            _context.PlayList.Add(playList);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPlayList", new { id = playList.Id }, playList);
        }

        // DELETE: api/PlayLists/5
        /// <summary>
        /// 删除歌单
        /// </summary>
        /// <remarks>
        /// DELETE: api/PlayLists/5
        /// 
        /// 需要管理员及以上权限
        /// 
        /// 若成功返回204
        /// </remarks>
        /// <param name="id">歌单ID</param>
        /// <returns>若成功返回204</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Root,Administrator")]
        public async Task<IActionResult> DeletePlayList(long id)
        {
            if (_context.PlayList == null)
            {
                return NotFound();
            }
            var playList = await _context.PlayList.FindAsync(id);
            if (playList == null)
            {
                return NotFound();
            }

            _context.PlayList.Remove(playList);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PlayListExists(long id)
        {
            return (_context.PlayList?.Any(e => e.Id == id)).GetValueOrDefault();
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
