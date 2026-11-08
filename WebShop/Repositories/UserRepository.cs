
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

        public async Task<User?> GetByEmail(string email)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                string query = "SELECT Id, Email, PasswordHash, FailedLoginAttempts, LockoutUntil FROM Users WHERE Email = @Email";

                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", email);

                await connection.OpenAsync();

                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return new User
                    {
                        Id = reader.GetInt32(0),
                        Email = reader.GetString(1),
                        PasswordHash = reader.GetString(2),
                        FailedLoginAttempts = reader.GetInt32(3),
                        LockoutUntil = reader.IsDBNull(4) ? null : reader.GetDateTime(4)

                    };
                }
            }
        }

        public async Task<int> Add(User user)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                string query = "INSERT INTO Users (Email, PasswordHash) VALUES (@Email, @PasswordHash) RETURNING Id";

                NpgsqlCommand command = new NpgsqlCommand(query, connection);

                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);

                await connection.OpenAsync();

                return (int)(await command.ExecuteScalarAsync())!;
            }
        }

        public async Task Update(User user)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                string query = @"
                UPDATE Users
                SET Email = @Email,
                    PasswordHash = @PasswordHash,
                    FailedLoginAttempts = @FailedLoginAttempts,
                    LockoutUntil = @LockoutUntil
                WHERE Id = @Id";

                NpgsqlCommand command = new NpgsqlCommand(query, connection);

                command.Parameters.AddWithValue("@Id", user.Id);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                command.Parameters.AddWithValue("@FailedLoginAttempts", user.FailedLoginAttempts);
                command.Parameters.AddWithValue("@LockoutUntil", (object?)user.LockoutUntil ?? DBNull.Value);

                await connection.OpenAsync();

                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
