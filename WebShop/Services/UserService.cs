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

            _userRepository.Add(newUser);
        }
    }
}
