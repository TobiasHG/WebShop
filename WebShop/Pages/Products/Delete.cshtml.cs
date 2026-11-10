using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebShop.Models;
using WebShop.Services;

namespace WebShop.Pages.Products
{
    [Authorize(Policy = Permissions.CanManipulateProducts)]
    public class DeleteModel : PageModel
    {

        private readonly ProductService _productService;
        public Product Product { get; set; }

        public DeleteModel(ProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            int ProductId = int.Parse(Request.Query["id"]);
            Product = await _productService.GetById(ProductId);

            if (Product == null)
            {
                return NotFound();
            }
            return Page();
        }

        [BindProperty]
        public int ProductId { get; set; }
        [BindProperty]
        public string ProductName { get; set; }
        public async Task<IActionResult> OnPostAsync()
        {

            Product = await _productService.GetById(ProductId);

            if (Product == null)
            {
                return NotFound();
            }

            if (Product.Name != ProductName)
            {
                TempData["Error"] = "Navnet matchede ikke. Prøv igen.";
                return RedirectToPage(new { id = ProductId });
            }

            await _productService.Delete(ProductId);
            return RedirectToPage("/Products/Index");
        }
    }
}
