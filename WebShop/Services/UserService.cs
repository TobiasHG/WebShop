using WebShop.Models;
using WebShop.Repositories;
using Microsoft.AspNetCore.Identity;

namespace WebShop.Services
{
    public class UserService
    {
        private UserRepository _userRepository;
        private CartRepository _cartRepository;
        private PasswordHasher<User> _passwordHasher;

        public UserService(UserRepository userRepository, CartRepository cartRepository)
        {
            _userRepository = userRepository;
            _cartRepository = cartRepository;
            _passwordHasher = new PasswordHasher<User>();
        }
        public async Task Add(string email, string password)
        {

            User newUser = new User
            {
                Email = email,
                RoleId = 1,
            };

            newUser.PasswordHash = _passwordHasher.HashPassword(newUser, password);

            int userId = await _userRepository.Add(newUser);

            await _cartRepository.CreateCart(userId);
        }
        public async Task<User?> Login(string email, string password)
        {
            User? user = await _userRepository.GetByEmail(email);

            if (user == null)
            {
                Console.WriteLine("User does not exist.");
                return null;
            }

            if (user.LockoutUntil.HasValue && user.LockoutUntil > DateTime.UtcNow)
            {
                Console.WriteLine("User is locked");
                return null;
            }

            PasswordVerificationResult result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

            if (result == PasswordVerificationResult.Failed)
            {
                user.FailedLoginAttempts++;

                if (user.FailedLoginAttempts >= 5)
                {
                    user.LockoutUntil = DateTime.UtcNow.AddMinutes(5);
                    user.FailedLoginAttempts = 0;
                    Console.WriteLine("User got locked out");

                }

                await _userRepository.Update(user);

                return null;
            }

            user.FailedLoginAttempts = 0;
            user.LockoutUntil = null;

            await _userRepository.Update(user);

            Console.WriteLine($"2 {user.Email} Attempts {user.FailedLoginAttempts}, lockout? {user.LockoutUntil}");

            return user;
        }
        public async Task<User?> GetByEmail(string email)
        {
            return await _userRepository.GetByEmail(email);
        }
        public async Task UpdatePassword(int userId, string passwordHash)
        {
            await _userRepository.UpdatePassword(userId, passwordHash);
        }

        public async Task<List<string>> GetPermissionsByUserId(int userId)
        {
            return await _userRepository.GetPermissionsByUserId(userId);
        }

        public async Task<User?> GetById(int id)
        {
            return await _userRepository.GetById(id);

        }
        public async Task<bool> HasPermission(int userid, string permission)
        {
            User? user = await _userRepository.GetById(userid);

            if (user == null)
            {
                return false;
            }

            List<string> permissions = await _userRepository.GetPermissionsByUserId(userid);

            return permissions.Contains(permission);
        }
    }
}
