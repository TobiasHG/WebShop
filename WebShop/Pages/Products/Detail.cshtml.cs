using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebShop.Models;
using WebShop.Services;

namespace WebShop.Pages.Products
{
    public class DetailModel : PageModel
    {
        private readonly ProductService _productService;

        public Product? Product { get; set; }

        public DetailModel(ProductService productService)
        {
            _productService = productService;
        }
        public async Task OnGet()
        {
            int ProductId = int.Parse(Request.Query["id"]);

            Product = await _productService.GetById(ProductId);

            if (Product == null)
            {
                NotFound();
            }

        }
    }
}
