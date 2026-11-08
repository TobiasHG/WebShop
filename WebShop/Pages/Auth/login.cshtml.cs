using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebShop.Models;
using WebShop.Services;

namespace WebShop.Pages.Auth
{
    public class loginModel : PageModel
    {
        private readonly ILogger<loginModel> _logger;
        private readonly UserService _userService;
        private readonly CartService _cartService;

        public loginModel(ILogger<loginModel> logger, UserService userService, CartService cartService)
        {
            _logger = logger;
            _userService = userService;
            _cartService = cartService;
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("Login attempt for email: {Email}", Email);
            User? user = await _userService.Login(Email, Password);

            if (user == null)
            {
                _logger.LogWarning("Login failed for email: {Email}", Email);
                ErrorMessage = "Forkert email eller adgangskode.";
                return Page();
            }

            int cartId = await _cartService.GetCartByUserId(user.Id);

            if (cartId == -1)
            {
                _logger.LogWarning("Failed to retrieve cart for user ID: {UserId}", user.Id);
                ErrorMessage = "Server fejl";
                return Page();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("CartId", cartId.ToString())
            };

            var identity = new ClaimsIdentity(claims, "Cookies");
            await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(identity));

            _logger.LogInformation("User {Email} logged in successfully with cart ID: {CartId}", Email, cartId);

            return RedirectToPage("/Index");
        }
    }
}
