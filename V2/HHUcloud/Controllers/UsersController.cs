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

    // PUT: api/Users/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
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

        return CreatedAtAction("GetUser", new { id = user.UserId }, user);
    }

    // DELETE: api/Users/5
    [HttpDelete("{id}")]
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

            user = await GetUserAsync(email);

            if (user != null)
            {
                string role = "Guest";
                if ((Role.Root | user.UserRole) > 0)
                {
                    role = Role.Root.ToString();
                }
                else if ((Role.Administrator | user.UserRole) > 0)
                {
                    role = Role.Administrator.ToString();
                }
                else
                {
                    if ((Role.User | user.UserRole) > 0)
                    {
                        role = Role.User.ToString();
                    }
                } // 设置用户权限

                //create claims details based on the user information
                var claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                    new Claim("UserId", user.UserId.ToString()),
                    new Claim("UserName", user.Name ?? $"用户{user.UserId}"),
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
    private bool UserExists(long id) => 
        (_context.User?.Any(e => e.UserId == id)).GetValueOrDefault();
    private bool UserExists(string email) =>
        (_context.User?.Any(e => e.Email == email)).GetValueOrDefault();
    private async Task<User?> GetUserAsync(string email) => 
        await _context.User.FirstOrDefaultAsync(u => u.Email == email);
}
