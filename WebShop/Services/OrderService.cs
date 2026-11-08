using WebShop.Models;
using WebShop.Repositories;

namespace WebShop.Services
{
    public class OrderService
    {
        private readonly OrderRepository orderRepository;

        public OrderService(OrderRepository orderRepository)
        {
            this.orderRepository = orderRepository;
        }

        public async Task<int> Add(Order order)
        {
            DateTime now = DateTime.UtcNow;
            order.OrderDate = now;
            order.TermsAcceptedAt = now;
            order.TotalAmount = order.OrderItems.Sum(item => item.UnitPrice * item.Quantity);

            return await orderRepository.Add(order);
        }

        public async Task<List<Order>> GetAll(int userId)
        {
            return await orderRepository.GetAll(userId);
        }
    }
}