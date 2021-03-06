using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdminController : BaseApiController
    {
        private readonly UserManager<AppUser> _usermanager;

        public AdminController(UserManager<AppUser> userManager) {
            _usermanager = userManager;

        }

        [Authorize(Policy="RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> getUsersWithRoles() {
            var users = await _usermanager.Users
                .Include(r => r.UserRoles)
                .ThenInclude(r => r.Role)
                .OrderBy(u => u.UserName)
                .Select(y => new {
                    y.Id,
                    Username = y.UserName,
                    Roles = y.UserRoles.Select(r => r.Role.Name).ToList()
                })
                .ToListAsync();
           return Ok(users);
        }

        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles) {
            var selectedRoles = roles.Split(",").ToArray();

            var user = await _usermanager.FindByNameAsync(username);
            if(user == null) return BadRequest("User not found");
            
            var userRoles = await _usermanager.GetRolesAsync(user);

            var result = await _usermanager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
            if(!result.Succeeded) return BadRequest("Failed to add to roles");

            result = await _usermanager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
            if(!result.Succeeded) return BadRequest("Failed to remove roles");

            return Ok(await _usermanager.GetRolesAsync(user));
        }

        [Authorize(Policy="ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public ActionResult getPhotosForModeration() {
           return Ok("Admins or moderators can see this!");
        }
    }
}