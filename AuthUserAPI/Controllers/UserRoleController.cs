using AuthUserAPI.Models;
using AuthUserAPI.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthUserAPI.Controllers
{
    [Route("userrole")]
    [ApiController]
    public class UserRoleController : ControllerBase
    {
        private readonly AuthuserContext _context;

        public UserRoleController(AuthuserContext context)
        {
            _context = context;
        }

        [HttpPost("assignrole")]
        public async Task<IActionResult> AssignRole([FromBody] NewUserRoleDto newUserRoleDto)
        {
            try
            {
                var user = await _context.Users.Include(u => u.Roles).SingleOrDefaultAsync(u => u.Id == newUserRoleDto.UserId);

                var role = await _context.Roles.FindAsync(newUserRoleDto.RoleId);

                if (user is null || role is null)
                {
                    return NotFound(new {message = "Hibás user vagy role id"});
                }

                bool isCreated = user.Roles.Any(r => r.Id == newUserRoleDto.RoleId);

                if (isCreated)
                {
                    return Conflict(new { message = "Ilyen szerepkör már van a usernek" });
                }

                user.Roles.Add(role);
                await _context.SaveChangesAsync();

                return StatusCode(201, new { message = "Sikeres összerendelés" });
            }
            catch (Exception exception)
            {

                return BadRequest(exception.Message);
            }
        }

        [HttpDelete("removerole")]
        public async Task<IActionResult> RemoveRole([FromBody] DeleteUserRoleDto deleteUserRoleDto)
        {
            

            try
            {
                var user = await _context.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Id == deleteUserRoleDto.UserId);
                var role = await _context.Roles.FindAsync(deleteUserRoleDto.RoleId);

                if (user is null || role is null)
                {
                    return NotFound(new { message = "Hibás user vagy role id" });
                }

                user.Roles.Remove(role);
                await _context.SaveChangesAsync();
                return StatusCode(200, new { message = "Sikeres törlés" });
            }
            catch (Exception exception)
            {

                return BadRequest(exception.Message);
            }
        }


        [HttpGet("allUserAllRoles")]
        public async Task<IActionResult> AllUserAllRoles()
        {
            var users = await _context.Users.Include(u => u.Roles).Select(u => new {Users = u.Name, UserRoles = u.Roles}).ToListAsync();

            return Ok(users);
        }


        [HttpGet("allUserAllRolesById")]
        public async Task<IActionResult> AllUserAllRolesById([FromQuery] Guid id)
        {
            var users = await _context.Users.
                Select(u => new
                {
                    u.Id,
                    u.Name,
                    UserRoles = u.Roles.Select(r => r.Name).ToList()
                }
                ).
                SingleOrDefaultAsync(u => u.Id == id);
                

            return Ok(users);
        }



    }
}
