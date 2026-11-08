using Microsoft.AspNetCore.Identity;
using WebShop.Models;
using WebShop.Repositories;

namespace WebShop.Services
{
    public class ProductService
    {
        private ProductRepository _productRepository;

        public ProductService(ProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task Add(string name, string description, decimal price, int stock, string imageUrl)
        {
            Product newProduct = new Product
            {
                Name = name,
                Price = price,
                Description = description,
                Stock = stock,
                ImageUrl = imageUrl
            };
            await _productRepository.Add(newProduct);
        }

        public async Task<List<Product>> GetAll()
        {
            return await _productRepository.GetAll();
        }

        public async Task<Product> GetById(int id)
        {
            return await _productRepository.GetById(id);
        }

        public async Task Update(Product product)
        {
            await _productRepository.Update(product);
        }

        public async Task Delete(int id)
        {
            await _productRepository.Delete(id);
        }

        public async Task<List<Product>> Search(string searchTerm)
        {
            return await _productRepository.Search(searchTerm);
        }
    }
}
