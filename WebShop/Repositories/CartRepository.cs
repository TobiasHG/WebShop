using Npgsql;
using WebShop.Models;

namespace WebShop.Repositories
{
    public class CartRepository
    {
        private string _connectionString;

        public CartRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<Cart> CreateCart(int userId)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                string query = "INSERT INTO Carts (UserId) VALUES (@UserId) RETURNING Id";

                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserId", userId);

                await connection.OpenAsync();

                int cartId = (int)(await command.ExecuteScalarAsync())!;

                return new Cart
                {
                    Id = cartId,
                    UserId = userId
                };
            }
        }

        public async Task AddItem(int cartId, int productId, int quantity)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = @"
                INSERT INTO CartItems (CartId, ProductId, Quantity)
                VALUES (@CartId, @ProductId, @Quantity)
                ON CONFLICT (CartId, ProductId)
                DO UPDATE SET Quantity = CartItems.Quantity + EXCLUDED.Quantity";

                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CartId", cartId);
                    command.Parameters.AddWithValue("@ProductId", productId);
                    command.Parameters.AddWithValue("@Quantity", quantity);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<List<CartItem>> GetCartItems(int cartId)
        {
            List<CartItem> cartItems = new List<CartItem>();

            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                string query = @"
            SELECT Id, CartId, ProductId, Quantity
            FROM CartItems
            WHERE CartId = @CartId";

                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@CartId", cartId);

                await connection.OpenAsync();

                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        cartItems.Add(new CartItem
                        {
                            Id = reader.GetInt32(0),
                            CartId = reader.GetInt32(1),
                            ProductId = reader.GetInt32(2),
                            Quantity = reader.GetInt32(3)
                        });
                    }
                }
            }

            return cartItems;
        }

        public async Task<int> GetCartByUserId(int userId)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                string query = "SELECT Id FROM Carts WHERE UserId = @UserId";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserId", userId);
                await connection.OpenAsync();
                var result = await command.ExecuteScalarAsync();
                if (result != null && int.TryParse(result.ToString(), out int cartId))
                {
                    return cartId;
                }
                else
                {
                    return -1;
                }
            }
        }

        public async Task ClearCart(int cartId)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                string query = "DELETE FROM CartItems WHERE CartId = @CartId";

                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CartId", cartId);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

    }
}
