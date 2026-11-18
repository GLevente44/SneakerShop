using AuthUserAPI.Models;
using AuthUserAPI.Models.DTOs;
using AuthUserAPI.Services;
using Microsoft.AspNetCore.Authorization;
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
        private readonly PasswordHasher _password;
        private readonly GenerateToken _generateToken;

        public UsersController(AuthuserContext context, PasswordHasher password, GenerateToken generateToken)
        {
            _context = context;
            _password = password;
            _generateToken = generateToken;
        }

        [HttpPost("newUser")]
        public async Task<IActionResult> NewUser([FromBody] NewUserDto newUserDto)
        {

            try
            {

                var (pass, salt) = _password.HashPassword(newUserDto.Password);

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Name = newUserDto.Name,
                    Email = newUserDto.Email,
                    Password = pass,
                    Salt = salt,
                    RegTime = DateTime.Now
                };

                if (user is not null)
                {
                    await _context.Users.AddAsync(user);
                    await _context.SaveChangesAsync();

                    return StatusCode(201, new {message = "Felvéve", result = user});

                }

                return StatusCode(400, new { message = "Hiba", result = user });
                }

            catch (Exception exception)
            {

                return BadRequest(exception.Message);
            }

        }

        [Authorize(Roles = "teacher,student")]
        [HttpGet("getAllUser")]
        public async Task<IActionResult> GetAllUser()
        {
            try
            {

                var users = await _context.Users.ToListAsync();
                if (users is not null)
                {
                    return StatusCode(200, new { message = "Lekérdezve", result = users });

                }
        
                return StatusCode(400, new { message = "Hiba", result = users });
                }
            catch (Exception exception)
            {

                return BadRequest(exception.Message);
            }

        }

        [HttpGet("byId")]
        public async Task<IActionResult> GetUserById([FromQuery] Guid id)
        {
            try
            {

                var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == id);

                if (user is not null)
                {
                    return StatusCode(200, new { message = "Lekérdezve", result = user });

                }
                return StatusCode(400, new { message = "Hiba", result = user });
            }

            catch (Exception exception)
            {

                return BadRequest(exception.Message);
            }

        }

        [HttpPut("updateUser")]
        public async Task<IActionResult> UpdateUser([FromQuery] Guid id, UpdateUserDto updateUserDto)
        {
            try
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

            catch (Exception exception)
            {

                return BadRequest(exception.Message);
            }

        }

        [HttpDelete("deleteUser")]
        public async Task<IActionResult> DeleteUser([FromQuery] Guid id)
        {
            try
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
            catch (Exception exception)
            {

                return BadRequest(exception.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userLoginDto)
        {
            try
            {

                var login = await _context.Users.Include(u => u.Roles).SingleOrDefaultAsync(u => u.Name == userLoginDto.Name);

                if (login is null)
                {
                    return StatusCode(400, new { message = "Nincs találat"});
                }

                var isValid = _password.Verify(userLoginDto.Password,login.Password,login.Salt);

                if (isValid)
                {
                    var newToken = _generateToken.Token(login.Id,login.Email,login.Roles);
                    return StatusCode(200, new { message = "Sikeres bejelentkezés", token = newToken});
                }

                return StatusCode(401, new { message = "Hibás név vagy jelszó"});
            }

            catch (Exception exception)
            {

                return BadRequest(exception.Message);
            }

        }


    }


}
