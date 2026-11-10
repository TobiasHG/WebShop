
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
                string query = @"
                INSERT INTO Users (Email, PasswordHash, RoleId)
                VALUES (@Email, @PasswordHash, @RoleId)
                RETURNING Id";

                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                    command.Parameters.AddWithValue("@RoleId", user.RoleId);

                    await connection.OpenAsync();

                    return (int)(await command.ExecuteScalarAsync())!;
                }
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

        public async Task UpdatePassword(int userId, string passwordHash)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                string query = @"
            UPDATE Users
            SET PasswordHash = @PasswordHash
            WHERE Id = @Id";

                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", userId);
                    command.Parameters.AddWithValue("@PasswordHash", passwordHash);

                    await connection.OpenAsync();

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<User?> GetById(int id)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                string query = @"
                SELECT Id, Email, PasswordHash, FailedLoginAttempts, LockoutUntil
                FROM Users
                WHERE Id = @Id";

                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    await connection.OpenAsync();

                    using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (!await reader.ReadAsync())
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
        }

        public async Task<List<string>> GetPermissionsByUserId(int userId)
        {
            List<string> permissions = new List<string>();

            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                string query = @"
                SELECT RolePermissions.Permission
                FROM Users
                JOIN Roles ON Roles.Id = Users.RoleId
                JOIN RolePermissions ON RolePermissions.RoleId = Roles.Id
                WHERE Users.Id = @UserId";

                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);

                    await connection.OpenAsync();

                    using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            permissions.Add(reader.GetString(0));
                        }
                    }
                }
            }

            return permissions;
        }
    }
}
