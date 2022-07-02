using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EMO_Cloud.Data;
using EMO_Cloud.Models;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EMO_Cloud.Tools;
using System.Text.Json.Nodes;

namespace EMO_Cloud.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        public IConfiguration _configuration;
        private readonly Context _context;

        public TokenController(IConfiguration config, Context context)
        {
            _configuration = config;
            _context = context;
        }

        /// <summary>
        /// 获取Token
        /// </summary>
        /// <remarks>
        /// POST: api/Token
        /// 
        /// FormData形式传输
        /// </remarks>
        /// <param name="email">邮箱地址</param>
        /// <param name="password">密码</param>
        /// <returns>若成功响应201并返回Token, 若失败响应400</returns>
        [HttpPost]
        public async Task<IActionResult> PostUser([FromForm] string email, [FromForm] string password)
        {
            if ((!string.IsNullOrWhiteSpace(email)) && (!string.IsNullOrWhiteSpace(password)))
            {
                password = MyTools.ComputeSHA256Hash(password + _configuration["Jwt:Salt"]); // 加盐并哈希

                var user = await GetUser(email, password);

                if (user != null)
                {
                    string role = "Guest";
                    if ((Role.Root | user.Role) > 0)
                    {
                        role = Role.Root.ToString();
                    }
                    else
                    {
                        if ((Role.Administrator | user.Role) > 0)
                        {
                            role = Role.Administrator.ToString();
                        }
                        else
                        {
                            if ((Role.User | user.Role) > 0)
                            {
                                role = Role.User.ToString();
                            }
                        }
                    } // 设置用户权限

                    //create claims details based on the user information
                    var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("UserId", user.Id.ToString()),
                        new Claim("UserName", user.Name),
                        new Claim(ClaimTypes.Role, role)
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                        _configuration["Jwt:Issuer"],
                        _configuration["Jwt:Audience"],
                        claims,
                        expires: DateTime.UtcNow.AddMinutes(10),
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
        /// 根据邮箱和密码查找用户
        /// </summary>
        /// <param name="email">用户邮箱地址</param>
        /// <param name="password">加盐哈希后的用户密码</param>
        /// <returns>查找结果, 用户不存在为null</returns>
        private async Task<User> GetUser(string email, string password)
        {
            return await _context.User.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
        }
    }
}
