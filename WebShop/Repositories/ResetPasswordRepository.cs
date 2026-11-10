using Npgsql;
using WebShop.Models;

namespace WebShop.Repositories
{
    public class ResetPasswordRepository
    {
        private string _connectionString;

        public ResetPasswordRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task Add(PasswordResetToken token)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                string query = "INSERT INTO PasswordResetTokens (UserId, TokenHash, ExpiresAt) VALUES (@UserId, @TokenHash, @ExpiresAt)";

                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserId", token.UserId);
                command.Parameters.AddWithValue("@TokenHash", token.TokenHash);
                command.Parameters.AddWithValue("@ExpiresAt", token.ExpiresAt);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<PasswordResetToken?> GetByHash(string tokenHash)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                string query = "SELECT Id, UserId, TokenHash, ExpiresAt FROM PasswordResetTokens WHERE TokenHash = @TokenHash";

                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@TokenHash", tokenHash);

                await connection.OpenAsync();

                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return new PasswordResetToken
                    {
                        Id = reader.GetInt32(0),
                        UserId = reader.GetInt32(1),
                        TokenHash = reader.GetString(2),
                        ExpiresAt = reader.GetDateTime(3)
                    };
                }
            }
        }

        public async Task DeleteByUserId(int userId)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                string query = "DELETE FROM PasswordResetTokens WHERE UserId = @UserId";

                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserId", userId);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}