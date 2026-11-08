using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebShop.Models;
using WebShop.Services;

namespace WebShop.Pages.Products
{
    public class AddModel : PageModel
    {

        private readonly ProductService _productService;

        public AddModel(ProductService productService)
        {
            _productService = productService;
        }

        public void OnGet()
        {
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
        public async Task OnPost()
        {
            await _productService.Add(Name, Description, Price, Stock, ImageUrl);
        }
    }
}
