
using Npgsql;
using WebShop.Models;

namespace WebShop.Repositories
{
    public class UserRepository
    {
        private string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Create
        public void Add(User user)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                string query = "INSERT INTO Users (Email, PasswordHash) VALUES (@Email, @PasswordHash)";

                NpgsqlCommand command = new NpgsqlCommand(query, connection);

                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);

                connection.Open();

                command.ExecuteNonQuery();
            }
        }
    }
}
