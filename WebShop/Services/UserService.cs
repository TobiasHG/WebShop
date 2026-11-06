using WebShop.Models;
using WebShop.Repositories;
using Microsoft.AspNetCore.Identity;

namespace WebShop.Services
{
    public class UserService
    {
        private UserRepository _userRepository;
        private PasswordHasher<User> _passwordHasher;

        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository;
            _passwordHasher = new PasswordHasher<User>();
        }
        public void Add(string email, string password)
        {

            User newUser = new User
            {
                Email = email,
            };

            newUser.PasswordHash = _passwordHasher.HashPassword(newUser, password);

            _userRepository.Add(newUser);
        }

        public User? Login(string email, string password)
        {
            User? user = _userRepository.GetByEmail(email);

            if (user == null)
            {
                return null;
            }

            PasswordVerificationResult result =
                _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

            return result == PasswordVerificationResult.Failed ? null : user;
        }
    }
}
