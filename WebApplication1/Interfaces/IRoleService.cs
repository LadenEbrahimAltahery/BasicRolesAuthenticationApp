﻿using RoleBasedBasicAuthentication.Models;
using RolesBasedAuthentication.Models;
namespace RoleBasedBasicAuthentication.Interfaces
{
public interface IRoleService
    {
        Task<IEnumerable<Role>> GetRolesAsync();
        Task AssignRoleToUserAsync(int userId, string roleName);
        Task<IEnumerable<Role>> GetUserRolesAsync(int userId);
    }
}
