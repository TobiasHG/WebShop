using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebShop.Models;
using WebShop.Repositories;
using WebShop.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebShop.Pages.Auth
{
    public class registerModel : PageModel
    {
        private UserService _userService;
        private PassOMeterService _passOMeterService;
        private CartRepository _cartRepository;
        public registerModel(UserService userService, PassOMeterService passOMeterService, CartRepository cartRepository)
        {
            _userService = userService;
            _passOMeterService = passOMeterService;
            _cartRepository = cartRepository;
        }
        public void OnGet()
        {
        }

        [BindProperty]
        public string? Email { get; set; }

        [BindProperty]
        public string Password { get; set; }
        public async Task<IActionResult> OnPost()
        {

            if (Email == null)
            {
                TempData["EmailError"] = "Du skal bruge en email.";
                return Page();
            }

            if (await _userService.GetByEmail(Email) != null)
            {
                TempData["EmailError"] = "Email allerede i brug.";
                return Page();
            }


            List<string> password_check = _passOMeterService.Verify(Password);

            if (password_check.Any())
            {
                TempData["PasswordError"] = string.Join(" ", password_check);
                return Page();
            }

            await _userService.Add(Email, Password);

            return Page();
        }
    }
}
