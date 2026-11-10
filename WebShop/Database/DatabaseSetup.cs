using Npgsql;
using WebShop.Models;

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
                        RoleId INTEGER NOT NULL REFERENCES Roles(Id),
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
        public void CreatePasswordResetTokenTable()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                string query = @"
        CREATE TABLE IF NOT EXISTS PasswordResetTokens
        (
            Id INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
            UserId INTEGER NOT NULL REFERENCES Users(Id) ON DELETE CASCADE,
            TokenHash CHAR(64) NOT NULL UNIQUE,
            ExpiresAt TIMESTAMPTZ NOT NULL
        );

        CREATE INDEX IF NOT EXISTS IX_PasswordResetTokens_UserId
            ON PasswordResetTokens(UserId);";

                NpgsqlCommand command = new NpgsqlCommand(query, connection);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
        public void CreateRoleTable()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                string query = @"
                    CREATE TABLE IF NOT EXISTS Roles
                    (
                        Id INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
                        Name VARCHAR(50) NOT NULL UNIQUE
                    )";

                NpgsqlCommand command = new NpgsqlCommand(query, connection);

                connection.Open();

                command.ExecuteNonQuery();
            }
        }
        public void CreateRolePermissionTable()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                string query = @"
                    CREATE TABLE IF NOT EXISTS RolePermissions
                    (
                        RoleId INTEGER NOT NULL REFERENCES Roles(Id) ON DELETE CASCADE,
                        Permission VARCHAR(100) NOT NULL,
                        PRIMARY KEY (RoleId, Permission)
                    )";

                NpgsqlCommand command = new NpgsqlCommand(query, connection);

                connection.Open();

                command.ExecuteNonQuery();
            }
        }
        public void ResetRoles()
        {
            Dictionary<string, string[]> roles = new Dictionary<string, string[]>
            {
                ["Customer"] = new[] { Permissions.CanResetPassword, Permissions.CanPlaceOrder },
                ["Administrator"] = new[] { Permissions.CanManipulateProducts },
            };

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlTransaction transaction = connection.BeginTransaction())
                {
                    new NpgsqlCommand("DELETE FROM RolePermissions", connection, transaction).ExecuteNonQuery();

                    foreach (KeyValuePair<string, string[]> role in roles)
                    {
                        NpgsqlCommand insertRole = new NpgsqlCommand(
                            "INSERT INTO Roles (Name) VALUES (@name) ON CONFLICT (Name) DO NOTHING",
                            connection, transaction);
                        insertRole.Parameters.AddWithValue("name", role.Key);
                        insertRole.ExecuteNonQuery();

                        foreach (string permission in role.Value)
                        {
                            NpgsqlCommand insertPermission = new NpgsqlCommand(
                                "INSERT INTO RolePermissions (RoleId, Permission) SELECT Id, @permission FROM Roles WHERE Name = @name",
                                connection, transaction);
                            insertPermission.Parameters.AddWithValue("permission", permission);
                            insertPermission.Parameters.AddWithValue("name", role.Key);
                            insertPermission.ExecuteNonQuery();
                        }
                    }

                    NpgsqlCommand deleteOld = new NpgsqlCommand(
                        "DELETE FROM Roles WHERE Name <> ALL(@names)", connection, transaction);
                    deleteOld.Parameters.AddWithValue("names", roles.Keys.ToArray());
                    deleteOld.ExecuteNonQuery();

                    transaction.Commit();
                }
            }
        }
        public void CreateTables()
        {
            CreateRoleTable();
            CreateRolePermissionTable();
            ResetRoles();

            CreateUserTable();
            CreateProductTable();
            CreateCartTable();
            CreateOrderTable();
            CreatePasswordResetTokenTable();

        }
    }
}
