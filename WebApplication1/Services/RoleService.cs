using RoleBasedBasicAuthentication.Interfaces;
using RoleBasedBasicAuthentication.Models;
using RolesBasedAuthentication.Models;
using System.Collections.Generic;
//using System.Threading.Tasks;

namespace RoleBasedBasicAuthentication.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        // Constructor injection of IRoleRepository into RoleService
        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        // Get all roles
        public async Task<IEnumerable<Role>> GetRolesAsync()
        {
            return await _roleRepository.GetRolesAsync();
        }

        // Assign a role to a user
        public async Task AssignRoleToUserAsync(int userId, string roleName)
        {
            // Call the repository method to assign the role to the user
            await _roleRepository.AssignRoleToUserAsync(userId, roleName);
        }

        // Get all roles assigned to a specific user
        public async Task<IEnumerable<Role>> GetUserRolesAsync(int userId)
        {
            return await _roleRepository.GetUserRolesAsync(userId);
        }
    }
}
