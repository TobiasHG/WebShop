using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebShop.Models;
using WebShop.Services;

namespace WebShop.Pages.Products
{
    public class ListModel : PageModel
    {
        private readonly ProductService _productService;
        public List<Product> Products { get; set; } = new List<Product>();

        public ListModel(ProductService productService)
        {
            _productService = productService;
        }

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; } = string.Empty;
        public async Task OnGet()
        {
            if (string.IsNullOrWhiteSpace(SearchTerm))
            {
                Products = await _productService.GetAll();
            }
            else
            {
                Products = await _productService.Search(SearchTerm);
            }
        }

    }
}
