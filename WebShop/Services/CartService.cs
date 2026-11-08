using WebShop.Models;
using WebShop.Repositories;

namespace WebShop.Services
{
    public class CartService
    {

        private readonly CartRepository _cartRepository;
        public CartService(CartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<Cart> CreateCart(int userId)
        {
            return await _cartRepository.CreateCart(userId);
        }

        public async Task AddItem(int cartId, int productId, int quantity)
        {
            if (quantity < 1)
            {
                return;
            }

            // TODO: validate productid

            await _cartRepository.AddItem(cartId, productId, quantity);
        }

        public async Task<List<CartItem>> GetCartItems(int cartId)
        {
            return await _cartRepository.GetCartItems(cartId);
        }

        public async Task<int> GetCartByUserId(int userId)
        {
            return await _cartRepository.GetCartByUserId(userId);
        }
    }
}
