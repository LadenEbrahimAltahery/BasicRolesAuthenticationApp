using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoleBasedBasicAuthentication.DTOs;
using RoleBasedBasicAuthentication.Interfaces;
using RoleBasedBasicAuthentication.DTOs;
using RoleBasedBasicAuthentication.Models;
using RoleBasedBasicAuthentication.Services;
using RolesBasedAuthentication.Models;

namespace RoleBasedBasicAuthenticationDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "BasicAuthentication")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/User/GetAllUsersAsync
        [HttpGet("GetAllUsersAsync")]
        [Authorize(Policy = "AdminOnly")] // Only Admin can view all users
        public async Task<ActionResult<IEnumerable<UserReadDTO>>> GetAllUsersAsync()
        {
            var users = await _userService.GetUsersAsync();

            var userDtos = users.Select(u => new UserReadDTO
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                // Password is excluded from the response for security
                Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
            }).ToList();

            return Ok(userDtos);
        }

        // GET: api/User/GetUserByIdAsync/1
        [HttpGet("GetUserByIdAsync/{id}")]
        [Authorize(Policy = "AdminOrUser")] // Admin and User can view individual user
        public async Task<ActionResult<UserReadDTO>> GetUserByIdAsync(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            var userDto = new UserReadDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
            };

            return Ok(userDto);
        }

        // POST: api/User/CreateUserAsync
        [HttpPost("CreateUserAsync")]
        [Authorize(Policy = "AdminOnly")] // Only Admin can create users
        public async Task<ActionResult<UserReadDTO>> CreateUserAsync([FromBody] UserCreateDTO userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Map DTO -> Entity
            var user = new User
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                Password = userDto.Password
            };

            user = await _userService.CreateUserAsync(user);

            // Assign roles if any
            foreach (var role in userDto.Roles)
            {
                await _userService.AssignRoleToUserAsync(user.Id, role);
            }

            // Map Entity -> DTO
            var createdUser = await _userService.GetUserByIdAsync(user.Id);
            var userReadDto = new UserReadDTO
            {
                Id = createdUser.Id,
                FirstName = createdUser.FirstName,
                LastName = createdUser.LastName,
                Email = createdUser.Email,
                Roles = createdUser.UserRoles.Select(ur => ur.Role.Name).ToList()
            };

            return CreatedAtAction(nameof(GetUserByIdAsync), new { id = user.Id }, userReadDto);
        }

        // PUT: api/User/UpdateUserAsync/1
        [HttpPut("UpdateUserAsync/{id}")]
        [Authorize(Policy = "AdminOnly")] // Only Admin can update users
        public async Task<IActionResult> UpdateUserAsync(int id, [FromBody] UserUpdateDTO userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != userDto.Id)
                return BadRequest("ID in URL doesn't match ID in payload.");

            // Map DTO -> Entity
            var user = new User
            {
                Id = userDto.Id,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                Password = userDto.Password
            };

            var updated = await _userService.UpdateUserAsync(user);
            if (!updated)
                return NotFound();

            // Update roles
            // For simplicity, remove all existing roles and assign new ones
            // In production, consider more sophisticated role management
            var existingRoles = await _userService.GetUserRolesAsync(user.Id);
            var rolesToRemove = existingRoles.Select(r => r.Name).Except(userDto.Roles).ToList();
            var rolesToAdd = userDto.Roles.Except(existingRoles.Select(r => r.Name)).ToList();

            // Note: Role removal not implemented; implement if necessary
            foreach (var role in rolesToAdd)
            {
                await _userService.AssignRoleToUserAsync(user.Id, role);
            }

            return NoContent();
        }

        // DELETE: api/User/DeleteUserAsync/1
        [HttpDelete("DeleteUserAsync/{id}")]
        [Authorize(Policy = "UserOnly")] // Only Admin can delete users
        public async Task<IActionResult> DeleteUserAsync(int id)
        {
            var deleted = await _userService.DeleteUserAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }

        // POST: api/User/1/assign-role
        [HttpPost("{id}/assign-role")]
        [Authorize(Policy = "AdminOnly")] // Only Admin can assign roles
        public async Task<IActionResult> AssignRoleToUserAsync(int id, [FromBody] string roleName)
        {
            try
            {
                await _userService.AssignRoleToUserAsync(id, roleName);
                return Ok($"Role '{roleName}' assigned to user with ID {id}.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}