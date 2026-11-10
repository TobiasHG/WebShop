using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

using WebShop.Models;
using WebShop.Services;

namespace WebShop.Pages.Order
{
    [Authorize(Policy = Permissions.CanPlaceOrder)]
    public class AddModel : PageModel
    {
        private readonly OrderService _orderService;
        private readonly PaymentService _paymentService;
        private readonly CartService _cartService;
        private readonly ProductService _productService;

        public AddModel(OrderService orderService, PaymentService paymentService, CartService cartService, ProductService productService)
        {
            _orderService = orderService;
            _paymentService = paymentService;
            _cartService = cartService;
            _productService = productService;
        }

        // Kontaktoplysninger
        [BindProperty]
        public string FullName { get; set; } = null!;
        [BindProperty]
        public string Email { get; set; } = null!;
        [BindProperty]
        public string? Phone { get; set; }

        // Leveringsadresse
        [BindProperty]
        public string Address { get; set; } = null!;
        [BindProperty]
        public string ZipCode { get; set; } = null!;
        [BindProperty]
        public string City { get; set; } = null!;
        [BindProperty]
        public string Country { get; set; } = "DK";

        // Betaling
        [BindProperty]
        public string CardName { get; set; } = null!;
        [BindProperty]
        public string CardNumber { get; set; } = null!;
        [BindProperty]
        public string CardExpiry { get; set; } = null!;
        [BindProperty]
        public string CardCvc { get; set; } = null!;

        // Andet
        [BindProperty]
        public string? Comment { get; set; }
        [BindProperty]
        public bool Terms { get; set; }

        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || !Terms)
            {
                Console.WriteLine("Model state is invalid or terms not accepted.");
                return Page();
            }

            string cartIdValue = User.FindFirst("CartId")!.Value;
            int CartId = int.Parse(cartIdValue);

            List<CartItem> cartItems = await _cartService.GetCartItems(CartId);

            List<OrderItem> items = new List<OrderItem>();
            foreach (CartItem cartItem in cartItems)
            {
                Product product = await _productService.GetById(cartItem.ProductId);
                items.Add(new OrderItem
                {
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = product.Price
                });
            }

            decimal totalAmount = items.Sum(item => item.UnitPrice * item.Quantity);

            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            bool paid = await _paymentService.ProcessPayment(CardName, CardNumber, CardExpiry, CardCvc, totalAmount);

            if (!paid)
            {
                ModelState.AddModelError(string.Empty, "Betalingen mislykkedes.");
                Console.WriteLine("Betalingen mislykkedes.");
                return Page();
            }

            Models.Order order = new Models.Order
            {
                UserId = userId,
                FullName = FullName,
                Email = Email,
                Phone = Phone,
                Address = Address,
                ZipCode = ZipCode,
                City = City,
                Country = Country,
                Comment = Comment,
                OrderItems = items
            };

            int orderId = await _orderService.Add(order);
            return RedirectToPage("/Order/Index");
        }
    }
}