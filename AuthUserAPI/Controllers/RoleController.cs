using AuthUserAPI.Models;
using AuthUserAPI.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthUserAPI.Controllers
{
    [Route("roles")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly AuthuserContext _context;

        public RoleController(AuthuserContext context)
        {
            _context = context;
        }

        [HttpPost("newRole")]
        public async Task<IActionResult> NewRole([FromBody] NewRoleDto newRoleDto)
        {
            try
            {
                var role = new Role
                {
                    Id = Guid.NewGuid(),
                    Name = newRoleDto.Name,
                };

                if (role is not null)
                {
                    await _context.Roles.AddAsync(role);
                    await _context.SaveChangesAsync();

                    return StatusCode(201, new { message = "Felvéve", result = role });

                }

                return StatusCode(400, new { message = "Hiba", result = role });

            }
            catch (Exception exception)
            {

                return BadRequest(exception.Message);
            }

        }

        [HttpGet("getAllRole")]
        public async Task<IActionResult> GetAllRole()
        {
            try
            {
                var roles = await _context.Roles.ToListAsync();
                if (roles is not null)
                {
                    return StatusCode(200, new { message = "Lekérdezve", result = roles });

                }
                return StatusCode(400, new { message = "Hiba", result = roles });

            }
            catch (Exception exception)
            {

                return BadRequest(exception.Message);
            }

        }

        [HttpGet("byId")]
        public async Task<IActionResult> GetRoleById([FromQuery] Guid id)
        {
            try
            {
                var role = await _context.Roles.SingleOrDefaultAsync(r => r.Id == id);

                if (role is not null)
                {
                    return StatusCode(200, new { message = "Lekérdezve", result = role });

                }
                return StatusCode(400, new { message = "Hiba", result = role });

            }
            catch (Exception exception)
            {

                return BadRequest(exception.Message);
            }

        }

        [HttpPut("updateRole")]
        public async Task<IActionResult> UpdateRole([FromQuery] Guid id, UpdateRoleDto updateRoleDto)
        {
            try
            {
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == id);

                if (role is not null)
                {
                    role.Name = updateRoleDto.Name;

                    _context.Roles.Update(role);
                    await _context.SaveChangesAsync();
                    return StatusCode(200, new { message = "Lekérdezve", result = role });

                }
                return StatusCode(400, new { message = "Nincs találat", result = role });

            }
            catch (Exception exception)
            {

                return BadRequest(exception.Message);
            }

        }

        [HttpDelete("deleteRole")]
        public async Task<IActionResult> DeleteRole([FromQuery] Guid id)
        {
            try
            {
                var role = await _context.Roles.FindAsync(id);
                if (role is not null)
                {
                    _context.Roles.Remove(role);
                    await _context.SaveChangesAsync();
                    return StatusCode(200, new { message = "Sikeres törlés", result = role });

                }

                return StatusCode(400, new { message = "Sikertelen törlés", result = role });

            }
            catch (Exception exception)
            {

                return BadRequest(exception.Message);
            }

        }



    }
}
