using Microsoft.AspNetCore.Identity;
using WebShop.Database;
using WebShop.Models;
using WebShop.Repositories;
using WebShop.Services;

namespace WebShop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAuthorization(options =>
            {
                foreach (string permission in Permissions.All)
                {
                    options.AddPolicy(permission, policy =>
                        policy.RequireClaim("permission", permission));
                }
            });

            builder.Logging.ClearProviders();

            builder.Logging.AddJsonConsole(options =>
            {
                options.IncludeScopes = true;
                options.UseUtcTimestamp = true;
                options.TimestampFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
            });

            builder.Services.AddRazorPages();

            builder.Services.AddHsts(options =>
            {
                options.MaxAge = TimeSpan.FromSeconds(63072000);
                options.IncludeSubDomains = true;
                options.Preload = true;
            });

            builder.Services.AddAuthentication("Cookies")
                .AddCookie("Cookies", options =>
                {
                    options.LoginPath = "/Auth/Login";
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Cookie.SameSite = SameSiteMode.Lax;
                });

            var DB_HOST = Environment.GetEnvironmentVariable("DB_HOST");
            var DB_PORT = Environment.GetEnvironmentVariable("DB_PORT");
            var DB_NAME = Environment.GetEnvironmentVariable("DB_NAME");
            var DB_USER = Environment.GetEnvironmentVariable("DB_USER");
            var DB_PASSWORD = Environment.GetEnvironmentVariable("DB_PASSWORD");
            var DB_SSLMODE = Environment.GetEnvironmentVariable("DB_SSLMODE");

            string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            if (!string.IsNullOrEmpty(DB_HOST) && !string.IsNullOrEmpty(DB_PORT) && !string.IsNullOrEmpty(DB_NAME) && !string.IsNullOrEmpty(DB_USER) && !string.IsNullOrEmpty(DB_PASSWORD))
            {

                var npgsqlBuilder = new Npgsql.NpgsqlConnectionStringBuilder
                {
                    Host = DB_HOST,
                    Port = int.Parse(DB_PORT),
                    Database = DB_NAME,
                    Username = DB_USER,
                    Password = DB_PASSWORD,
                    SslMode = Enum.Parse<Npgsql.SslMode>(DB_SSLMODE, true)
                };

                connectionString = npgsqlBuilder.ConnectionString;

            }
            else if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("Database connection string is not configured.");
            }

            builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

            builder.Services.AddScoped(_ => new UserRepository(connectionString));
            builder.Services.AddScoped<UserService>();

            builder.Services.AddScoped(_ => new ProductRepository(connectionString));
            builder.Services.AddScoped<ProductService>();

            builder.Services.AddScoped(_ => new CartRepository(connectionString));
            builder.Services.AddScoped<CartService>();

            builder.Services.AddScoped(_ => new OrderRepository(connectionString));
            builder.Services.AddScoped<OrderService>();

            builder.Services.AddScoped(_ => new ResetPasswordRepository(connectionString));
            builder.Services.AddScoped<ResetPasswordService>();

            builder.Services.AddScoped<PaymentService>();

            builder.Services.AddScoped<PassOMeterService>();

            var app = builder.Build();

            DatabaseSetup databaseSetup = new DatabaseSetup(connectionString);
            databaseSetup.CreateTables();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.Use(async (context, next) =>
            {
                context.Response.Headers["X-Content-Type-Options"] = "nosniff";
                context.Response.Headers["X-Frame-Options"] = "DENY";
                context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
                context.Response.Headers["Content-Security-Policy"] =
                    "default-src 'self'; " +
                    "script-src 'self'; " +
                    "style-src 'self' 'unsafe-inline'; " +
                    "img-src 'self' data:; " +
                    "object-src 'none'; " +
                    "base-uri 'self'; " +
                    "form-action 'self'; " +
                    "frame-ancestors 'none'";

                await next();
            });

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapRazorPages().WithStaticAssets();

            app.Run();
        }
    }
}