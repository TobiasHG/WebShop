using WebShop.Database;
using WebShop.Repositories;
using WebShop.Services;

namespace WebShop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();

            // HSTS: 2 år, alle subdomæner og preload
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

            string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            Console.WriteLine($"Using connection string: {connectionString}");

            builder.Services.AddScoped(_ => new UserRepository(connectionString));
            builder.Services.AddScoped<UserService>();

            builder.Services.AddScoped(_ => new ProductRepository(connectionString));
            builder.Services.AddScoped<ProductService>();

            builder.Services.AddScoped(_ => new CartRepository(connectionString));
            builder.Services.AddScoped<CartService>();

            builder.Services.AddScoped(_ => new OrderRepository(connectionString));
            builder.Services.AddScoped<OrderService>();

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