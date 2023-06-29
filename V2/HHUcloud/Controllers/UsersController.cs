using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HHUcloud.Data;
using HHUcloud.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using HHUcloud.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace HHUcloud.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UsersController : ControllerBase
{
    private readonly IConfiguration _configuration;
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

        var list = await _context.User.ToListAsync();

        foreach (var user in list)
        {
            await FillPlaylistAsync(user);
        }

        return list;
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

        await FillPlaylistAsync(user);

        return user;
    }

    /// <summary>
    /// 获取当前用户信息
    /// </summary>
    /// <remarks>
    /// GET: api/Users/Info
    /// 
    /// 需要Token
    /// </remarks>
    /// <returns>若成功响应201并返回当前用户信息; 若失败响应404(数据库表为空), 400(授权失败)</returns>
    [HttpGet("Info")]
    [Authorize(Roles = "Root,Administrator,User,Guest")]
    public async Task<ActionResult<User>> Info()
    {
        if (_context.User == null)
        {
            return NotFound();
        }
        var _user = await GetUserAsync(GetUserId());

        if (_user == null)
        {
            return NotFound();
        }

        User user = JsonSerializer.Deserialize<User>(JsonSerializer.Serialize(_user)); //Deep Copy

        user.UserPassword = string.Empty; // hide information
        user.SecurityKey = string.Empty; // hide information

        await FillPlaylistAsync(user);

        return user;
    }

    /// <summary>
    /// 找回密码
    /// </summary>
    /// <remarks>
    /// POST: api/Users/FindBackPassword
    /// 
    /// 允许匿名访问
    /// 
    /// JSON形式传输
    /// 
    /// 若成功响应201并返回用户信息; 若失败返回500(格式错误), 400(安全代码错误), 404(用户不存在)
    /// 
    /// </remarks>
    [HttpPost("FindBackPassword")]
    [AllowAnonymous]
    public async Task<ActionResult<User>> FindBackPassword(User user)
    {
        if (_context.User == null)
        {
            return Problem("Entity set 'Context.User'  is null.");
        }
        string email = user.Email;
        string newPassword = user.UserPassword;
        string securityKey = user.SecurityKey;
        if (!UserExists(email))
        {
            return NotFound();
        }
        var _user = await _context.User.FirstOrDefaultAsync(e => e.Email == email);

        if (_user.SecurityKey == securityKey)
        {
            _user.UserPassword = Sha256Tool.ComputeSHA256Hash(newPassword + _configuration["Jwt:Salt"]);
            _context.User.Update(_user);
            await _context.SaveChangesAsync();
        }
        else
        {
            return Problem("SecurityKey is wrong.");
        }
        user = JsonSerializer.Deserialize<User>(JsonSerializer.Serialize(_user)); //Deep Copy

        user.UserPassword = string.Empty; // hide information
        user.SecurityKey = string.Empty; // hide information

        await FillPlaylistAsync(user);

        return user;
    }

    /// <summary>
    /// 更新用户信息
    /// </summary>
    /// <remarks>
    /// PUT: api/Users/5
    /// 
    /// 管理员及以上权限
    /// </remarks>
    /// <param name="id">用户ID</param>
    /// <param name="user">用户信息</param>
    [HttpPut("{id}")]
    [Authorize(Roles = "Root,Administrator")]
    public async Task<IActionResult> PutUser(long id, User user)
    {
        if (id != user.UserId)
        {
            return BadRequest();
        }

        _context.Entry(user).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!UserExists(id))
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
    [Authorize(Roles = "Root,Administrator,User")]
    public async Task<ActionResult<User>> ChangePassword([FromForm] string oldPassword, [FromForm] string newPassword)
    {
        if (_context.User == null)
        {
            return Problem("Entity set 'Context.User'  is null.");
        }

        long id = GetUserId();

        if (!UserExists(id))
        {
            return NotFound();
        }
        var _user = await _context.User.FirstOrDefaultAsync(e => e.UserId == id);

        if (_user.UserPassword == Sha256Tool.ComputeSHA256Hash(oldPassword + _configuration["Jwt:Salt"]))
        {
            _user.UserPassword = Sha256Tool.ComputeSHA256Hash(newPassword + _configuration["Jwt:Salt"]);
            //_context.User.Add(user);
            await _context.SaveChangesAsync();
        }
        else
        {
            return Problem("Old password is wrong.");
        }
        User user = JsonSerializer.Deserialize<User>(JsonSerializer.Serialize(_user)); //Deep Copy

        user.UserPassword = string.Empty; // hide information
        user.SecurityKey = string.Empty; // hide information

        return user;
    }

    /// <summary>
    /// 修改用户信息
    /// </summary>
    /// <remarks>
    /// POST: api/Users/UpdateInfo
    /// 
    /// JSON形式传输
    /// 
    /// 若成功响应200并返回用户信息; 若失败返回500(格式错误), 400(安全代码错误), 404(用户不存在)
    /// 
    /// </remarks>
    /// <param name="user">用户信息</param>
    [HttpPost("UpdateInfo")]
    [Authorize(Roles = "Root,Administrator,User,Guest")]
    public async Task<ActionResult<User>> UpdateInfo(User user)
    {
        if (_context.User == null)
        {
            return Problem("Entity set 'Context.User' is null.");
        }
        if (user == null)
        {
            return Problem("Parameter 'user' is null.");
        }

        long id = GetUserId();

        if (!UserExists(id))
        {
            return NotFound("用户不存在");
        }
        var _user = await _context.User!.FirstAsync(e => e.UserId == id);

        _user.Name = user.Name;
        _user.ProfilePhoto = user.ProfilePhoto;
        _user.Birthday = user.Birthday;
        _user.Email = user.Email;
        
        _context.User.Update(_user);
        await _context.SaveChangesAsync();

        user = JsonSerializer.Deserialize<User>(JsonSerializer.Serialize(_user)!)!; //Deep Copy

        user.UserPassword = string.Empty; // hide information
        user.SecurityKey = string.Empty; // hide information

        return user;
    }
    /// <summary>
    /// 获取用户按时间倒序的播放记录
    /// </summary>
    /// <remarks>
    /// GET: api/Users/RecentPlay
    /// 
    /// 返回歌曲列表
    /// </remarks>
    /// <param name="size">记录条数, 默认为100</param>
    /// <returns>最近size条记录, 若记录条数不足, 返回所有记录</returns>
    [HttpGet("RecentPlay")]
    [Authorize(Roles = "Root,Administrator,User")]
    public async Task<ActionResult<List<Song>>> RecentPlay(int size = 100)
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

        var songIds = _context.PlayRecord?.ToList()
            .Where(e => e.UserId == user.UserId)
            .OrderByDescending(e => e.Played) // 按时间近->远顺序排列
            .Select(p => p.SongId)
            .Take(size)
            .ToList();

        var songs = new List<Song>();

        foreach (var songId in songIds)
        {
            var song = _context.Song.Find(songId);
            await FillSongInfoAsync(song);
            songs.Add(song);
        }

        return songs;
    }

    /// <summary>
    /// 用户注册
    /// </summary>
    /// <remarks>
    /// POST: api/Users/Regist
    /// 
    /// 允许匿名访问
    /// 
    /// JSON形式传参
    /// 
    /// 可选: securityKey 用于确定注册用户权限
    /// 
    /// 若成功将响应201并返回用户信息; 若失败返回500(格式错误), 400(邮箱已存在/数据库表为空)
    /// 
    /// </remarks>
    /// <param name="user">用户信息(邮箱和密码必选)</param>
    /// <returns>若成功将响应201并返回用户信息; 若失败返回500(格式错误), 400(邮箱已存在/数据库表为空)</returns>
    [HttpPost("Regist")]
    [AllowAnonymous]
    public async Task<ActionResult<User>> Regist(User user)
    {
        if (_context.User == null)
        {
            return Problem("Entity set 'Context.User'  is null.");
        }

        if (UserExists(user.Email))
        {
            return Problem("User is already exists.");
        }

        if (!string.IsNullOrEmpty(user.SecurityKey))
        {
            if (user.SecurityKey == _configuration["Jwt:Root"])
            {
                user.UserRole = Role.Root;
            }
            else if (user.SecurityKey == _configuration["Jwt:Administrator"])
            {
                user.UserRole = Role.Administrator;
            }
            else
            {
                user.UserRole = Role.User;
            }
        }
        else
        {
            user.UserRole = Role.User;
        }

        user.UserPassword = Sha256Tool.ComputeSHA256Hash(
            user.UserPassword + 
            _configuration["Jwt:Salt"]);

        user.Created = DateTime.UtcNow;

        user.SecurityKey = Sha256Tool.ComputeSHA256Hash(
            user.Created
            + user.UserPassword
            + _configuration["Jwt:Salt"]);

        _context.User.Add(user);
        await _context.SaveChangesAsync();

        var playlist = new Playlist()
        {
            UserId = user.UserId,
            Title = "我的收藏",
            Created = DateTime.UtcNow,
        };
        _context.Playlist.Add(playlist);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetUser", new { id = user.UserId }, user);
    }

    // DELETE: api/Users/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Root,Administrator")]
    public async Task<IActionResult> DeleteUser(long id)
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

        _context.User.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// 获取Token
    /// </summary>
    /// <remarks>
    /// POST: api/Users/Token
    /// 
    /// JSON形式传输
    /// </remarks>
    /// <param name="user">用户信息(邮箱和密码必选)</param>
    /// <returns>若成功响应201并返回Token, 若失败响应400</returns>
    [HttpPost("Token")]
    [AllowAnonymous]
    public async Task<IActionResult> Token(User user)
    {
        string email = user.Email;
        string password = user.UserPassword;
        if ((!string.IsNullOrWhiteSpace(email)) && (!string.IsNullOrWhiteSpace(password)))
        {
            password = Sha256Tool.ComputeSHA256Hash(password + _configuration["Jwt:Salt"]); // 加盐并哈希

            var _user = await GetUserAsync(email);

            if (_user != null)
            {
                if (password != _user.UserPassword)
                {
                    return BadRequest("Wrong email or password");
                }
                string role = "Guest";
                if ((Role.Root | _user.UserRole) > 0)
                {
                    role = Role.Root.ToString();
                }
                else if ((Role.Administrator | _user.UserRole) > 0)
                {
                    role = Role.Administrator.ToString();
                }
                else
                {
                    if ((Role.User | _user.UserRole) > 0)
                    {
                        role = Role.User.ToString();
                    }
                } // 设置用户权限

                //create claims details based on the user information
                var claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                    new Claim("UserId", _user.UserId.ToString()),
                    new Claim("UserName", _user.Name ?? $"用户{_user.UserId}"),
                    new Claim(ClaimTypes.Role, role)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    _configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    claims,
                    expires: DateTime.UtcNow.AddDays(1),
                    signingCredentials: signIn);

                return Ok(new JwtSecurityTokenHandler().WriteToken(token));
            }
            else
            {
                return BadRequest("Invalid credentials");
            }
        }
        else
        {
            return BadRequest();
        }
    }

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

    private async Task FillPlaylistAsync(User user)
    {
        if (user == null)
        {
            return;
        }

        var playlists = _context.Playlist
            .Where(e => e.UserId == user.UserId);

        user.PlaylistIds = await playlists
            .Select(e => e.PlaylistId)
            .ToListAsync();

        user.PlaylistTitles = await playlists
            .Select(e => e.Title)
            .ToListAsync();
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
    private bool UserExists(long id) => 
        (_context.User?.Any(e => e.UserId == id)).GetValueOrDefault();
    private bool UserExists(string email) =>
        (_context.User?.Any(e => e.Email == email)).GetValueOrDefault();
    private async Task<User?> GetUserAsync(long id) =>
        await _context.User.FirstOrDefaultAsync(u => u.UserId == id);
    private async Task<User?> GetUserAsync(string email) => 
        await _context.User.FirstOrDefaultAsync(u => u.Email == email);
}
