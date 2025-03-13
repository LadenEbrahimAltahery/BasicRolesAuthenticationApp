using RoleBasedBasicAuthentication.Models;
using RoleBasedBasicAuthentication.Interfaces;
using RolesBasedAuthentication.Models;

namespace RoleBasedBasicAuthenticationDemo.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public UserService(IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        // User CRUD Operations
        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _userRepository.GetUsersAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetUserByIdAsync(id);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            return await _userRepository.CreateUserAsync(user);
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            return await _userRepository.UpdateUserAsync(user);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            return await _userRepository.DeleteUserAsync(id);
        }

        public async Task<User?> ValidateUserAsync(string email, string password)
        {
            return await _userRepository.ValidateUserAsync(email, password);
        }

        // Role Management
        public async Task<IEnumerable<Role>> GetRolesAsync()
        {
            return await _roleRepository.GetRolesAsync();
        }

        public async Task AssignRoleToUserAsync(int userId, string roleName)
        {
            await _roleRepository.AssignRoleToUserAsync(userId, roleName);
        }

        public async Task<IEnumerable<Role>> GetUserRolesAsync(int userId)
        {
            return await _roleRepository.GetUserRolesAsync(userId);
        }
    }
}
