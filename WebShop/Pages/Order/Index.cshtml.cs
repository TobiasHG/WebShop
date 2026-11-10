using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using WebShop.Models;
using WebShop.Services;

namespace WebShop.Pages.Order
{
    [Authorize(Policy = Permissions.CanPlaceOrder)]
    public class IndexModel : PageModel
    {

        private readonly OrderService _orderService;
        private readonly ProductService _productService;
        public IndexModel(OrderService orderService, ProductService productService)
        {
            _orderService = orderService;
            _productService = productService;
        }

        public List<Models.Order> Orders { get; set; } = new List<Models.Order>();
        public Dictionary<int, string> ProductIdToName = new Dictionary<int, string>();

        public async Task OnGetAsync()
        {

            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            Orders = await _orderService.GetAll(userId);

            foreach (var order in Orders)
            {
                foreach (var item in order.OrderItems)
                {
                    if (!ProductIdToName.ContainsKey(item.ProductId))
                    {
                        var product = await _productService.GetById(item.ProductId);
                        ProductIdToName[item.ProductId] = product?.Name ?? "Unknown Product";
                    }
                }
            }

        }
    }
}
