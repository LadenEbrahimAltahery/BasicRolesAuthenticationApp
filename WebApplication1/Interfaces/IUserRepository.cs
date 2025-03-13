using RoleBasedBasicAuthentication.Models;
using RolesBasedAuthentication.Models;

namespace RoleBasedBasicAuthentication.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<User> CreateUserAsync(User user);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int id);
        Task<User?> ValidateUserAsync(string email, string password);
    }
}
