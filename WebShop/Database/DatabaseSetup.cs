
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
                        PasswordHash VARCHAR(255) NOT NULL,
                        FailedLoginAttempts INTEGER NOT NULL DEFAULT 0,
                        LockoutUntil TIMESTAMPTZ NULL
                    )";

                NpgsqlCommand command = new NpgsqlCommand(query, connection);

                connection.Open();

                command.ExecuteNonQuery();
            }
        }
        public void CreateProductTable()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                string query = @"
                    CREATE TABLE IF NOT EXISTS Products
                    (
                        Id INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
                        Name VARCHAR(255) NOT NULL,
                        Description TEXT NOT NULL,
                        Price DECIMAL(18, 2) NOT NULL,
                        Stock INTEGER NOT NULL,
                        ImageUrl VARCHAR(255) NOT NULL
                    )";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
        public void CreateCartTable()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                string query = @"
                    CREATE TABLE IF NOT EXISTS Carts
                    (
                        Id INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
                        UserId INTEGER NOT NULL UNIQUE REFERENCES Users(Id) ON DELETE CASCADE
                    );

                    CREATE TABLE IF NOT EXISTS CartItems
                    (
                        Id INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
                        CartId INTEGER NOT NULL REFERENCES Carts(Id) ON DELETE CASCADE,
                        ProductId INTEGER NOT NULL REFERENCES Products(Id) ON DELETE CASCADE,
                        Quantity INTEGER NOT NULL DEFAULT 1 CHECK (Quantity > 0),
                        UNIQUE (CartId, ProductId)
                    );";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void CreateOrderTable()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                string query = @"
                CREATE TABLE IF NOT EXISTS Orders
                (
                    Id INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
                    UserId INTEGER NOT NULL REFERENCES Users(Id),
                    OrderDate TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    TotalAmount DECIMAL(18, 2) NOT NULL,

                    FullName TEXT NOT NULL,
                    Email TEXT NOT NULL,
                    Phone TEXT NULL,

                    Address TEXT NOT NULL,
                    ZipCode VARCHAR(4) NOT NULL,
                    City TEXT NOT NULL,
                    Country CHAR(2) NOT NULL DEFAULT 'DK',

                    Comment TEXT NULL,
                    TermsAcceptedAt TIMESTAMPTZ NOT NULL
                );

                CREATE TABLE IF NOT EXISTS OrderItems
                (
                    Id INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
                    OrderId INTEGER NOT NULL REFERENCES Orders(Id) ON DELETE CASCADE,
                    ProductId INTEGER NOT NULL REFERENCES Products(Id),
                    Quantity INTEGER NOT NULL CHECK (Quantity > 0),
                    UnitPrice DECIMAL(18, 2) NOT NULL
                );";

                NpgsqlCommand command = new NpgsqlCommand(query, connection);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void CreateTables()
        {
            CreateUserTable();
            CreateProductTable();
            CreateCartTable();
            CreateOrderTable();
        }
    }
}
