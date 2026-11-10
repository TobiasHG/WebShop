using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebShop.Services;

namespace WebShop.Pages.Auth
{
    public class RequestResetPasswordModel : PageModel
    {
        private readonly ResetPasswordService _resetPasswordService;

        public RequestResetPasswordModel(ResetPasswordService resetPasswordService)
        {
            _resetPasswordService = resetPasswordService;
        }

        [BindProperty]
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; } = string.Empty;

        public bool Submitted { get; set; }
        public string? ResetLink { get; set; }

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

            ResetLink = await _resetPasswordService.RequestPasswordReset(Email.Trim());

            Submitted = true;

            return Page();
        }
    }
}
