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

        // POST: api/Token
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostUser(User user)
        {
            if (user != null && user.Email != null && user.Password != null)
            {
                string password = user.Password + _configuration["Jwt:Salt"];

                var findedUser = await GetUser(user.Name, MyTools.ComputeSHA256Hash(password));

                if (findedUser != null)
                {
                    string role = "Guest";
                    if ((Role.Root | findedUser.Role) > 0)
                    {
                        role = Role.Root.ToString();
                    }
                    else
                    {
                        if ((Role.Administrator | findedUser.Role) > 0)
                        {
                            role = Role.Administrator.ToString();
                        }
                        else
                        {
                            if ((Role.User | findedUser.Role) > 0)
                            {
                                role = Role.User.ToString();
                            }
                        }
                    }
                    //create claims details based on the user information
                    var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("UserId", findedUser.Id.ToString()),
                        new Claim("UserName", findedUser.Name),
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

        private async Task<User> GetUser(string name, string password)
        {
            return await _context.User.FirstOrDefaultAsync(u => u.Name == name && u.Password == password);
        }
    }
}
