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
        /// GET: api/Users
        /// 返回所有用户信息
        /// 需要提供Token, 管理员及以上用户可用
        /// </summary>
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
        /// GET: api/Users/5
        /// 返回指定id用户信息
        /// 需要提供Token, 管理员及以上用户可用
        /// </summary>
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
        /// POST: api/Users/login
        /// 需要提供Token
        /// </summary>
        /// <returns>若成功响应201并返回用户信息(隐去密码和安全码), 若失败响应400(授权失败/数据库表为空), 404(用户不存在)</returns>
        [HttpPost]
        [Authorize(Roles = "Root,Administrator,User")]
        [Route("Login")]
        public async Task<ActionResult<User>> Login()
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

            user.Password = string.Empty; // hide information
            user.SecurityKey = string.Empty; // hide information

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        /// <summary>
        /// POST: api/Users/Regist
        /// 用户注册, JSON形式传输, 不需要的属性应置空, 不能缺失
        /// 需要(加*为必须填写): Age, Email(*), Name(*), Password(*), ProfilePhoto
        /// 可选: securityKey 复用以确定注册用户权限
        /// </summary>
        /// <param name="user">用户信息</param>
        /// <returns>若成功响应201并返回用户信息; 若失败返回500(JSON格式错误), 400(邮箱已存在/数据库表为空)</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("Regist")]
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
                if (user.SecurityKey == _configuration["RootCode"])
                {
                    user.Role = Role.Root;
                }
                else if (user.SecurityKey == _configuration["Jwt:Administrator"])
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

            //user.Id = await _context.User.MaxAsync(e => e.Id) + 1;
            user.Password = MyTools.ComputeSHA256Hash(user.Password + _configuration["Jwt:Salt"]);
            user.CreatedDate = DateTime.Now.ToString();
            user.SecurityKey = MyTools.ComputeSHA256Hash(user.CreatedDate + _configuration["Jwt:Salt"]);
            
            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
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
            return long.Parse(this.User.Claims.First(i => i.Type == "UserId").Value);
        }
    }
}
