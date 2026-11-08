using Npgsql;
using WebShop.Models;

namespace WebShop.Repositories
{
    public class OrderRepository
    {
        private readonly string connectionString;

        public OrderRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<int> Add(Order order)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (NpgsqlTransaction transaction = await connection.BeginTransactionAsync())
                {
                    string orderQuery = @"
                        INSERT INTO Orders
                            (UserId, OrderDate, TotalAmount, FullName, Email, Phone,
                             Address, ZipCode, City, Country, Comment, TermsAcceptedAt)
                        VALUES
                            (@UserId, @OrderDate, @TotalAmount, @FullName, @Email, @Phone,
                             @Address, @ZipCode, @City, @Country, @Comment, @TermsAcceptedAt)
                        RETURNING Id;";

                    int orderId;
                    using (NpgsqlCommand command = new NpgsqlCommand(orderQuery, connection, transaction))
                    {
                        command.Parameters.AddWithValue("UserId", order.UserId);
                        command.Parameters.AddWithValue("OrderDate", order.OrderDate);
                        command.Parameters.AddWithValue("TotalAmount", order.TotalAmount);
                        command.Parameters.AddWithValue("FullName", order.FullName);
                        command.Parameters.AddWithValue("Email", order.Email);
                        command.Parameters.AddWithValue("Phone", (object?)order.Phone ?? DBNull.Value);
                        command.Parameters.AddWithValue("Address", order.Address);
                        command.Parameters.AddWithValue("ZipCode", order.ZipCode);
                        command.Parameters.AddWithValue("City", order.City);
                        command.Parameters.AddWithValue("Country", order.Country);
                        command.Parameters.AddWithValue("Comment", (object?)order.Comment ?? DBNull.Value);
                        command.Parameters.AddWithValue("TermsAcceptedAt", order.TermsAcceptedAt);

                        orderId = (int)(await command.ExecuteScalarAsync())!;
                    }

                    string itemQuery = @"
                        INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice)
                        VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice);";

                    foreach (OrderItem item in order.OrderItems)
                    {
                        using (NpgsqlCommand command = new NpgsqlCommand(itemQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("OrderId", orderId);
                            command.Parameters.AddWithValue("ProductId", item.ProductId);
                            command.Parameters.AddWithValue("Quantity", item.Quantity);
                            command.Parameters.AddWithValue("UnitPrice", item.UnitPrice);
                            await command.ExecuteNonQueryAsync();
                        }
                    }

                    await transaction.CommitAsync();
                    return orderId;
                }
            }
        }

        public async Task<List<Order>> GetAll(int userId)
        {
            List<Order> orders = new List<Order>();
            Dictionary<int, Order> ordersById = new Dictionary<int, Order>();

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string orderQuery = @"
            SELECT Id, UserId, OrderDate, TotalAmount, FullName, Email, Phone,
                   Address, ZipCode, City, Country, Comment, TermsAcceptedAt
            FROM Orders
            WHERE UserId = @UserId
            ORDER BY OrderDate DESC;";

                using (NpgsqlCommand command = new NpgsqlCommand(orderQuery, connection))
                {
                    command.Parameters.AddWithValue("UserId", userId);

                    using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Order order = new Order
                            {
                                Id = reader.GetInt32(0),
                                UserId = reader.GetInt32(1),
                                OrderDate = reader.GetDateTime(2),
                                TotalAmount = reader.GetDecimal(3),
                                FullName = reader.GetString(4),
                                Email = reader.GetString(5),
                                Phone = reader.IsDBNull(6) ? null : reader.GetString(6),
                                Address = reader.GetString(7),
                                ZipCode = reader.GetString(8),
                                City = reader.GetString(9),
                                Country = reader.GetString(10),
                                Comment = reader.IsDBNull(11) ? null : reader.GetString(11),
                                TermsAcceptedAt = reader.GetDateTime(12)
                            };

                            orders.Add(order);
                            ordersById[order.Id] = order;
                        }
                    }
                }

                string itemQuery = @"
            SELECT oi.Id, oi.OrderId, oi.ProductId, oi.Quantity, oi.UnitPrice
            FROM OrderItems oi
            JOIN Orders o ON o.Id = oi.OrderId
            WHERE o.UserId = @UserId;";

                using (NpgsqlCommand command = new NpgsqlCommand(itemQuery, connection))
                {
                    command.Parameters.AddWithValue("UserId", userId);

                    using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int orderId = reader.GetInt32(1);

                            if (ordersById.TryGetValue(orderId, out Order? order))
                            {
                                order.OrderItems.Add(new OrderItem
                                {
                                    Id = reader.GetInt32(0),
                                    OrderId = orderId,
                                    ProductId = reader.GetInt32(2),
                                    Quantity = reader.GetInt32(3),
                                    UnitPrice = reader.GetDecimal(4)
                                });
                            }
                        }
                    }
                }
            }

            return orders;
        }
    }
}