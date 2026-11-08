using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebShop.Models;
using WebShop.Services;

namespace WebShop.Pages.Products
{
    public class UpdateModel : PageModel
    {
        private readonly ProductService _productService;

        public Product? Product { get; set; }

        public UpdateModel(ProductService productService)
        {
            _productService = productService;
        }
        public async Task OnGet()
        {
            int ProductId = int.Parse(Request.Query["id"]);

            Product = await _productService.GetById(ProductId);
        }

        [BindProperty]
        public string Name { get; set; }
        [BindProperty]
        public string Description { get; set; }
        [BindProperty]
        public decimal Price { get; set; }
        [BindProperty]
        public int Stock { get; set; }
        [BindProperty]
        public string ImageUrl { get; set; }

        public async Task<IActionResult> OnPost()
        {
            int ProductId = int.Parse(Request.Query["id"]);
            Product = await _productService.GetById(ProductId);

            if (Product == null)
            {
                return NotFound();
            }

            Product.Name = Name;
            Product.Description = Description;
            Product.Price = Price;
            Product.Stock = Stock;
            Product.ImageUrl = ImageUrl;

            await _productService.Update(Product);

            return RedirectToPage("/Products/Detail", new { id = ProductId });
        }
    }
}
