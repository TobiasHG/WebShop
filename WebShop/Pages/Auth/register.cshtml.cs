using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebShop.Models;
using WebShop.Repositories;
using WebShop.Services;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;

namespace WebShop.Pages.Auth
{
    public class registerModel : PageModel
    {
        private readonly ILogger<registerModel> _logger;
        private UserService _userService;
        private PassOMeterService _passOMeterService;
        private CartRepository _cartRepository;
        public registerModel(ILogger<registerModel> logger, UserService userService, PassOMeterService passOMeterService, CartRepository cartRepository)
        {
            _logger = logger;
            _userService = userService;
            _passOMeterService = passOMeterService;
            _cartRepository = cartRepository;
        }
        public void OnGet()
        {
        }

        [BindProperty]
        [Required(ErrorMessage = "Email er påkrævet.")]
        public string? Email { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Adgangskode er påkrævet.")]
        public string Password { get; set; }
        public async Task<IActionResult> OnPost()
        {

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for register attempt with email: {Email}", Email);
                return Page();
            }

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
