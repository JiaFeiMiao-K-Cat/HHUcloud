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

        // GET: api/Users
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

        // GET: api/Users/5
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

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Root,Administrator,User")]
        [Route("Show")]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            if (_context.User == null)
            {
                return Problem("Entity set 'UserContext.User'  is null.");
            }
            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // POST: api/Users/Regist
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [AllowAnonymous]
        [Route("Regist")]
        public async Task<ActionResult<User>> Regist(User user)
        {
            if (_context.User == null)
            {
                return Problem("Entity set 'UserContext.User'  is null.");
            }

            if (UserExists(user.Email))
            {
                return Problem("User is already exists.");
            }

            user.Id = await _context.User.MaxAsync(e => e.Id) + 1;
            user.Password = MyTools.ComputeSHA256Hash(user.Password + _configuration["Jwt:Salt"]);
            user.SecurityKey = MyTools.ComputeSHA256Hash(DateTime.Now.ToString());
#if DEBUG
            user.Role = Role.Administrator;
#else
            user.Role = Role.User;
#endif
            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        private bool UserExists(string email)
        {
            return (_context.User?.Any(e => e.Email == email)).GetValueOrDefault();
        }
    }
}
