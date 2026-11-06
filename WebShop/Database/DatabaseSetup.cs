
using Npgsql;

namespace WebShop.Database
{
    public class DatabaseSetup
    {
        private string connectionString;

        public DatabaseSetup(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void CreateUserTable()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                string query = @"
                    CREATE TABLE IF NOT EXISTS Users
                    (
                        Id INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
                        Email VARCHAR(255) NOT NULL,
                        PasswordHash VARCHAR(255) NOT NULL
                    )";

                NpgsqlCommand command = new NpgsqlCommand(query, connection);

                connection.Open();

                command.ExecuteNonQuery();
            }
        }

        public void CreateTables()
        {
            CreateUserTable();
        }
    }
}
