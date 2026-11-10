using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
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
        [Required(ErrorMessage = "Email er påkrævet.")]
        public string Email { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Adgangskode er påkrævet.")]
        public string Password { get; set; }

        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("Login attempt for email: {Email}", Email);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for email: {Email}", Email);
                return Page();
            }

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

            string UserId = user.Id.ToString();
            List<string> UserPermissions = await _userService.GetPermissionsByUserId(user.Id);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, UserId),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("CartId", cartId.ToString()),
            };

            foreach (string permission in UserPermissions)
            {
                claims.Add(new Claim("Permission", permission));
            }

            var identity = new ClaimsIdentity(claims, "Cookies");
            await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(identity));

            _logger.LogInformation("User {Email} logged in successfully with cart ID: {CartId}", Email, cartId);

            return RedirectToPage("/Index");
        }
    }
}
