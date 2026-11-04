using AuthUserAPI.Models;
using AuthUserAPI.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthUserAPI.Controllers
{
    [Route("users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AuthuserContext _context;

        public UsersController(AuthuserContext context)
        {
            _context = context;
        }

        [HttpPost("newUser")]
        public async Task<IActionResult> NewUser([FromBody] NewUserDto newUserDto)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = newUserDto.Name,
                Email = newUserDto.Email,
                Password = newUserDto.Password,
            };

            if (user is not null)
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                return StatusCode(201, new {message = "Felvéve", result = user});

            }

            return StatusCode(400, new { message = "Hiba", result = user });

        }

        [HttpGet("getAllUser")]
        public async Task<IActionResult> GetAllUser()
        {
            var users = await _context.Users.ToListAsync();
            if (users is not null)
            {
                return StatusCode(200, new { message = "Lekérdezve", result = users });

            }
            return StatusCode(400, new { message = "Hiba", result = users });

        }

        [HttpGet("byId")]
        public async Task<IActionResult> GetUserById([FromQuery] Guid id)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == id);

            if (user is not null)
            {
                return StatusCode(200, new { message = "Lekérdezve", result = user });

            }
            return StatusCode(400, new { message = "Hiba", result = user });

        }

        [HttpPut("updateUser")]
        public async Task<IActionResult> UpdateUser([FromQuery] Guid id, UpdateUserDto updateUserDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user is not null)
            {
                user.Name = updateUserDto.Name;
                user.Email = updateUserDto.Email;
                user.Password = updateUserDto.Password;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return StatusCode(200, new { message = "Lekérdezve", result = user });

            }
            return StatusCode(400, new { message = "Nincs találat", result = user });

        }

        [HttpDelete("deleteUser")]
        public async Task<IActionResult> DeleteUser([FromQuery] Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user is not null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return StatusCode(200, new { message = "Sikeres törlés", result = user });

            }

            return StatusCode(400, new { message = "Sikertelen törlés", result = user });

        }



    }


}
