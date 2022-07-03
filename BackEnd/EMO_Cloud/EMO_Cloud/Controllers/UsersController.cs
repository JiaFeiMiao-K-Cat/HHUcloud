using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EMO_Cloud.Data;
using EMO_Cloud.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using EMO_Cloud.Tools;
using Swashbuckle.Swagger.Annotations;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace EMO_Cloud.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsersController : ControllerBase
    {
        public IConfiguration _configuration;
        private readonly Context _context;

        public UsersController(IConfiguration config, Context context)
        {
            _configuration = config;
            _context = context;
        }

        /// <summary>
        /// 获取所有用户信息
        /// </summary>
        /// <remarks>
        /// GET: api/Users
        /// 
        /// 需要管理员及以上权限
        /// </remarks>
        /// <returns>若成功响应201并返回所有用户信息; 若失败响应404(数据库表为空), 400(授权失败)</returns>
        [HttpGet]
        [Authorize(Roles = "Root,Administrator")]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
          if (_context.User == null)
          {
              return NotFound();
          }
            return await _context.User.ToListAsync();
        }

        /// <summary>
        /// 获取指定id用户信息
        /// </summary>
        /// <remarks>
        /// GET: api/Users/5
        /// 
        /// 需要管理员及以上权限
        /// </remarks>
        /// <returns>若成功响应201并返回所有用户信息; 若失败响应404(数据库表为空), 400(授权失败)</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Root,Administrator")]
        public async Task<ActionResult<User>> GetUser(long id)
        {
          if (_context.User == null)
          {
              return NotFound();
          }
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        /// <summary>
        /// 获取用户本人信息
        /// </summary>
        /// <remarks>
        /// POST: api/Users/Info
        /// </remarks>
        /// <returns>若成功响应201并返回用户信息(隐去密码和安全码), 若失败响应400(授权失败/数据库表为空), 404(用户不存在)</returns>
        [HttpPost("Info")]
        public async Task<ActionResult<User>> Info()
        {
            if (_context.User == null)
            {
                return Problem("Entity set 'Context.User'  is null.");
            }
            long id = GetUserId();

            User _user = await _context.User.FindAsync(id);

            if (_user == null)
            {
                return NotFound();
            }

            User user = JsonSerializer.Deserialize<User>(JsonSerializer.Serialize(_user)); //Deep Copy

            user.Password = string.Empty; // hide information
            user.SecurityKey = string.Empty; // hide information

            user.PlayLists = _context.PlayList?
                .Where(e => e.UserId == user.Id).Select(p => p.Id).ToList();

            user.HistoryPlay = _context.PlayRecord?
                .Where(e => e.UserId == user.Id).OrderBy(p => p.LastTime).ToList();
            // sort by Last time

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        /// <summary>
        /// 获取用户歌单列表
        /// </summary>
        /// <remarks>
        /// POST: api/Users/PlayList
        /// 
        /// **当前用户不存在歌单时会自动创建一个**
        /// 
        /// 返回含有歌单名称(Item1)和歌单ID(Item2)的元组列表
        /// </remarks>
        /// <returns>若成功响应201并返回用户歌单列表, 若失败响应400(授权失败/数据库表为空), 404(用户不存在)</returns>
        [HttpPost("PlayList")]
        public async Task<ActionResult<List<Tuple<string, long>>>> PlayList()
        {
            if (_context.User == null)
            {
                return Problem("Entity set 'Context.User'  is null.");
            }

            long id = GetUserId();

            User user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            if (!_context.PlayList.Any(e => e.UserId == user.Id))
            {
                PlayList playlist = new PlayList();
                playlist.Id = 0;
                playlist.ListTitle = "我的收藏";
                playlist.UserId = user.Id;
                _context.PlayList.Add(playlist);
                _context.SaveChanges();
            }

            return _context.PlayList?
                .Where(e => e.UserId == user.Id)
                .Select(p => new Tuple<string, long>(p.ListTitle, p.Id)).ToList();
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <remarks>
        /// POST: api/Users/Regist
        /// 
        /// 允许匿名访问
        /// 
        /// FormData形式传参
        /// 
        /// 可选: securityKey 用于确定注册用户权限
        /// 
        /// 若成功将响应201并返回用户信息; 若失败返回500(格式错误), 400(邮箱已存在/数据库表为空)
        /// 
        /// </remarks>
        /// <param name="email">邮箱地址</param>
        /// <param name="name">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="securityKey">安全代码</param>
        /// <returns>若成功将响应201并返回用户信息; 若失败返回500(格式错误), 400(邮箱已存在/数据库表为空)</returns>
        [HttpPost("Regist")]
        [AllowAnonymous]
        public async Task<ActionResult<User>> Regist([FromForm] string email, [FromForm] string name, [FromForm] string password, [FromForm] string? securityKey)
        {
            if (_context.User == null)
            {
                return Problem("Entity set 'Context.User'  is null.");
            }

            if (UserExists(email))
            {
                return Problem("User is already exists.");
            }

            User user = new User();
            if (!string.IsNullOrEmpty(securityKey))
            {
                if (securityKey == _configuration["RootCode"])
                {
                    user.Role = Role.Root;
                }
                else if (securityKey == _configuration["Jwt:Administrator"])
                {
                    user.Role = Role.Administrator;
                }
                else
                {
                    user.Role = Role.User;
                }
            }
            else
            {
                user.Role = Role.User;
            }
            user.Email = email;
            user.Name = name;
            user.Password = MyTools.ComputeSHA256Hash(password + _configuration["Jwt:Salt"]);
            user.CreatedDate = DateTime.Now.ToString();
            user.SecurityKey = MyTools.ComputeSHA256Hash(user.CreatedDate + password + _configuration["Jwt:Salt"]);
            
            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        /// <summary>
        /// 找回密码
        /// </summary>
        /// <remarks>
        /// POST: api/Users/FindBackPassword
        /// 
        /// 允许匿名访问
        /// 
        /// FormData形式传输
        /// 
        /// 若成功响应201并返回用户信息; 若失败返回500(格式错误), 400(安全代码错误), 404(用户不存在)
        /// 
        /// </remarks>
        /// <param name="email">邮箱地址</param>
        /// <param name="newPassword">新密码</param>
        /// <param name="securityKey">安全代码</param>
        [HttpPost("FindBackPassword")]
        [AllowAnonymous]
        public async Task<ActionResult<User>> FindBackPassword([FromForm] string email, [FromForm] string newPassword, [FromForm] string securityKey)
        {
            if (_context.User == null)
            {
                return Problem("Entity set 'Context.User'  is null.");
            }

            if (!UserExists(email))
            {
                return NotFound();
            }
            var _user = await _context.User.FirstOrDefaultAsync(e => e.Email == email);

            if (_user.SecurityKey == securityKey)
            {
                _user.Password = MyTools.ComputeSHA256Hash(newPassword + _configuration["Jwt:Salt"]);
                //_context.User.Add(user);
                await _context.SaveChangesAsync();
            }
            else
            {
                return Problem("SecurityKey is wrong.");
            }
            User user = JsonSerializer.Deserialize<User>(JsonSerializer.Serialize(_user)); //Deep Copy

            user.Password = string.Empty; // hide information
            user.SecurityKey = string.Empty; // hide information

            return user;
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <remarks>
        /// POST: api/Users/ChangePassword
        /// 
        /// FormData形式传输
        /// 
        /// 若成功响应201并返回用户信息; 若失败返回500(格式错误), 400(安全代码错误), 404(用户不存在)
        /// 
        /// </remarks>
        /// <param name="oldPassword">原始密码</param>
        /// <param name="newPassword">新密码</param>
        [HttpPost("ChangePassword")]
        public async Task<ActionResult<User>> ChangePassword([FromForm] string oldPassword, [FromForm] string newPassword)
        {
            if (_context.User == null)
            {
                return Problem("Entity set 'Context.User'  is null.");
            }

            long id = GetUserId();

            if ((!_context.User?.Any(e => e.Id == id)).GetValueOrDefault())
            {
                return NotFound();
            }
            var _user = await _context.User.FirstOrDefaultAsync(e => e.Id == id);

            if (_user.Password == MyTools.ComputeSHA256Hash(oldPassword + _configuration["Jwt:Salt"]))
            {
                _user.Password = MyTools.ComputeSHA256Hash(newPassword + _configuration["Jwt:Salt"]);
                //_context.User.Add(user);
                await _context.SaveChangesAsync();
            }
            else
            {
                return Problem("SecurityKey is wrong.");
            }
            User user = JsonSerializer.Deserialize<User>(JsonSerializer.Serialize(_user)); //Deep Copy

            user.Password = string.Empty; // hide information
            user.SecurityKey = string.Empty; // hide information

            return user;
        }
        /// <summary>
        /// 获取用户按时间倒序的播放记录
        /// </summary>
        /// <remarks>
        /// POST: api/Users/RecentPlay
        /// 
        /// FormData形式传输
        /// 
        /// 返回含有歌名(Item1), 歌曲ID(Item2)和播放时间(Item3)的元组列表
        /// </remarks>
        /// <param name="size">记录条数</param>
        /// <returns>最近size条记录, 若记录条数不足, 返回所有记录</returns>
        [HttpPost("RecentPlay")]
        public async Task<ActionResult<List<Tuple<string, long, DateTime>>>> RecentPlay([FromForm] int size)
        {
            if (_context.User == null)
            {
                return Problem("Entity set 'Context.User'  is null.");
            }

            long id = GetUserId();

            User user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return _context.PlayRecord?.ToList()
                .Where(e => e.UserId == user.Id)
                .OrderByDescending(e => e.LastTime) // 按时间近->远顺序排列
                .Select(p => new Tuple<string, long, DateTime>(
                    (_context.Song.Find(p.SongId)?? new Song()).Title, 
                    p.SongId, p.LastTime)).Take(size)
                .ToList();
        }

        /// <summary>
        /// 修改用户名
        /// </summary>
        /// <remarks>
        /// POST: api/Users/ChangeUserName
        /// 
        /// FormData形式传输
        /// 
        /// 若成功响应201并返回用户信息; 若失败返回500(格式错误), 400(安全代码错误), 404(用户不存在)
        /// 
        /// </remarks>
        /// <param name="newName">新用户名</param>
        [HttpPost("ChangeUserName")]
        public async Task<ActionResult<User>> ChangeUserName([FromForm] string newName)
        {
            if (_context.User == null)
            {
                return Problem("Entity set 'Context.User'  is null.");
            }

            long id = GetUserId();

            if ((!_context.User?.Any(e => e.Id == id)).GetValueOrDefault())
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(newName))
            {
                return BadRequest("Illegal parameter");
            }

            var _user = await _context.User.FirstOrDefaultAsync(e => e.Id == id);
            _user.Name = newName;
            //_context.User.Add(user);
            await _context.SaveChangesAsync();
            User user = JsonSerializer.Deserialize<User>(JsonSerializer.Serialize(_user)); //Deep Copy

            user.Password = string.Empty; // hide information
            user.SecurityKey = string.Empty; // hide information

            return user;
        }

        /// <summary>
        /// 该邮箱地址的用户是否存在
        /// </summary>
        /// <param name="email">邮箱地址</param>
        /// <returns>用户是否存在</returns>
        private bool UserExists(string email)
        {
            return (_context.User?.Any(e => e.Email == email)).GetValueOrDefault();
        }

        /// <summary>
        /// copy from: https://stackoverflow.com/questions/50580232/get-userid-from-jwt-on-all-controller-methods
        /// </summary>
        /// <returns>当前Token的用户ID</returns>
        protected long GetUserId()
        {
            if (this.User.Claims.FirstOrDefault(i => i.Type == "UserId") == null)
            {
                return -1;
            } // Without Token
            return long.Parse(this.User.Claims.First(i => i.Type == "UserId").Value);
        }
    }
}
