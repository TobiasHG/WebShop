using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebShop.Models;
using WebShop.Services;

namespace WebShop.Pages.Auth
{
    public class registerModel : PageModel
    {
        private UserService _userService;

        public registerModel(UserService userService)
        {
            _userService = userService;
        }
        public void OnGet()
        {
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }
        public void OnPost()
        {

            Console.WriteLine($"Email: {Email}, Password: {Password}");
            _userService.Add(Email, Password);
        }
    }
}
