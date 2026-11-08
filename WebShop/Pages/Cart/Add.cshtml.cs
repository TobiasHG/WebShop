using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using WebShop.Services;

namespace WebShop.Pages.Cart
{
    public class AddModel : PageModel
    {

        private readonly CartService _cartService;
        public AddModel(CartService cartService)
        {
            _cartService = cartService;
        }

        public void OnGet()
        {
        }


        [BindProperty]
        public int ProductId { get; set; }

        [BindProperty]
        public int Quantity { get; set; }

        [Authorize]
        public async Task<IActionResult> OnPostAsync()
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var cartId = User.FindFirst("CartId")?.Value;

            await _cartService.AddItem(int.Parse(cartId), ProductId, Quantity);

            return RedirectToPage("/Cart/Index");
        }
    }
}
