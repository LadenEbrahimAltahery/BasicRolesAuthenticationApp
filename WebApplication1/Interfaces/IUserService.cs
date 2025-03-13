using RoleBasedBasicAuthentication.Models;
using RolesBasedAuthentication.Models;

namespace RoleBasedBasicAuthentication.Interfaces
{
    public interface IUserService
    {
        // User CRUD Operations
        Task<IEnumerable<User>> GetUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<User> CreateUserAsync(User user);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int id);
        Task<User?> ValidateUserAsync(string email, string password);
        Task AssignRoleToUserAsync(int userId, string roleName);
        Task<IEnumerable<Role>> GetUserRolesAsync(int userId);

    }
}
