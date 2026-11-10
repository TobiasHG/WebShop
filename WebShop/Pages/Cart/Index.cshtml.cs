using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebShop.Models;
using WebShop.Services;
using WebShop.Dtos;

namespace WebShop.Pages.Cart
{
    [Authorize(Policy = Permissions.CanPlaceOrder)]
    public class IndexModel : PageModel
    {
        private readonly CartService _cartService;
        private readonly ProductService _productService;

        public IndexModel(CartService cartService, ProductService productService)
        {
            _cartService = cartService;
            _productService = productService;
        }

        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public List<ProductInCart> CartItemProducts { get; set; } = new List<ProductInCart>();
        public async Task<IActionResult> OnGetAsync()
        {
            string? userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return RedirectToPage("/Auth/Login");
            }

            CartItems = await _cartService.GetCartItems(userId);

            foreach (var item in CartItems)
            {
                var product = await _productService.GetById(item.ProductId);

                if (product != null)
                {
                    CartItemProducts.Add(new ProductInCart
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Quantity = item.Quantity,
                        ProductImage = product.ImageUrl,
                        ProductPrice = product.Price,
                    });
                }
            }

            return Page();
        }
    }
}
