
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

        public User? GetByEmail(string email)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                string query = "SELECT Id, Email, PasswordHash FROM Users WHERE Email = @Email";

                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", email);

                connection.Open();

                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return new User
                    {
                        Id = reader.GetInt32(0),
                        Email = reader.GetString(1),
                        PasswordHash = reader.GetString(2)
                    };
                }
            }
        }

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
