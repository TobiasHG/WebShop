using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebShop.Services;

namespace WebShop.Pages.Auth
{
    public class ResetPasswordModel : PageModel
    {
        private readonly ResetPasswordService _resetPasswordService;
        private readonly PassOMeterService _passOMeterService;

        public ResetPasswordModel(ResetPasswordService resetPasswordService, PassOMeterService passOMeterService)
        {
            _resetPasswordService = resetPasswordService;
            _passOMeterService = passOMeterService;
        }

        [BindProperty(SupportsGet = true)]
        public string? Token { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Skriv en ny adgangskode.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Gentag adgangskoden.")]
        [Compare(nameof(NewPassword), ErrorMessage = "Adgangskoderne er ikke ens.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;

        public bool Success { get; set; }

        public void OnGet()
        {
            Response.Headers["Referrer-Policy"] = "no-referrer";
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Response.Headers["Referrer-Policy"] = "no-referrer";

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (string.IsNullOrWhiteSpace(Token))
            {
                return Page();
            }

            if (!string.IsNullOrEmpty(NewPassword))
            {
                foreach (string error in _passOMeterService.Verify(NewPassword))
                {
                    ModelState.AddModelError(nameof(NewPassword), error);
                }
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            bool success = await _resetPasswordService.ResetPassword(Token, NewPassword);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Linket er ugyldigt eller udløbet.");
                return Page();
            }

            Success = true;
            return Page();
        }
    }
}