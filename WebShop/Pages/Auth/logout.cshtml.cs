using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebShop.Pages.Auth
{
    public class logoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            return RedirectToPage("/Index");
        }

        [Authorize]
        public async Task<IActionResult> OnPostAsync()
        {
            // TODO: put i user service
            await HttpContext.SignOutAsync("Cookies");

            return RedirectToPage("/Index");
        }
    }
}
