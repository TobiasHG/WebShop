
using Npgsql;
using WebShop.Models;

namespace WebShop.Repositories
{
    public class ProductRepository
    {
        private string _connectionString;

        public ProductRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task Add(Product product)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                string query = "INSERT INTO Products (Name, Description, Price, Stock, ImageUrl) VALUES (@Name, @Description, @Price, @Stock, @ImageUrl)";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@Name", product.Name);
                command.Parameters.AddWithValue("@Description", product.Description);
                command.Parameters.AddWithValue("@Price", product.Price);
                command.Parameters.AddWithValue("@Stock", product.Stock);
                command.Parameters.AddWithValue("@ImageUrl", product.ImageUrl);
                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<List<Product>> GetAll()
        {
            List<Product> products = new List<Product>();
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                string query = "SELECT Id, Name, Description, Price, Stock, ImageUrl FROM Products";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                await connection.OpenAsync();
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Product product = new Product
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Description = reader.GetString(2),
                            Price = reader.GetDecimal(3),
                            Stock = reader.GetInt32(4),
                            ImageUrl = reader.GetString(5)
                        };
                        products.Add(product);
                    }
                }
            }
            return products;
        }

        public async Task<Product> GetById(int id)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                string query = "SELECT Id, Name, Description, Price, Stock, ImageUrl FROM Products WHERE Id = @Id";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);
                await connection.OpenAsync();
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        Product product = new Product
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Description = reader.GetString(2),
                            Price = reader.GetDecimal(3),
                            Stock = reader.GetInt32(4),
                            ImageUrl = reader.GetString(5)
                        };
                        return product;
                    }
                }
            }
            return null;
        }

        public async Task Update(Product product)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                string query = "UPDATE Products SET Name = @Name, Description = @Description, Price = @Price, Stock = @Stock, ImageUrl = @ImageUrl WHERE Id = @Id";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", product.Id);
                command.Parameters.AddWithValue("@Name", product.Name);
                command.Parameters.AddWithValue("@Description", product.Description);
                command.Parameters.AddWithValue("@Price", product.Price);
                command.Parameters.AddWithValue("@Stock", product.Stock);
                command.Parameters.AddWithValue("@ImageUrl", product.ImageUrl);
                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task Delete(int id)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                string query = "DELETE FROM Products WHERE Id = @Id";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);
                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<List<Product>> Search(string searchTerm)
        {
            List<Product> products = new List<Product>();
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                string query = "SELECT Id, Name, Description, Price, Stock, ImageUrl FROM Products WHERE Name ILIKE @SearchTerm OR Description ILIKE @SearchTerm";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");
                await connection.OpenAsync();
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Product product = new Product
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Description = reader.GetString(2),
                            Price = reader.GetDecimal(3),
                            Stock = reader.GetInt32(4),
                            ImageUrl = reader.GetString(5)
                        };
                        products.Add(product);
                    }
                }
            }
            return products;
        }
    }
}
