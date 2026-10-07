
using Application;
using infrastructure;
using infrastructure.Persistence;
using Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Talenty.OpenApi;

namespace Talenty
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");
            Console.WriteLine($"IsDevelopment: {builder.Environment.IsDevelopment()}");

            builder.Configuration.Sources.Clear();
            
            builder.Configuration
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

            if (builder.Environment.IsDevelopment())
            {
                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                Console.WriteLine($"ApplicationData path: {appDataPath}");
                
                var userSecretsPath = Path.Combine(
                    appDataPath,
                    "Microsoft", "UserSecrets", "81a8bf9d-ecf3-4fae-8eda-a2db0b79fec1", "secrets.json");
                
                Console.WriteLine($"Looking for user secrets at: {userSecretsPath}");
                Console.WriteLine($"File exists: {File.Exists(userSecretsPath)}");
                
                if (File.Exists(userSecretsPath))
                {
                    builder.Configuration.AddJsonFile(userSecretsPath, optional: true, reloadOnChange: true);
                    Console.WriteLine($"Loaded user secrets from: {userSecretsPath}");
                }
                else
                {
                    Console.WriteLine($"User secrets file not found at: {userSecretsPath}");
                }
            }
            else
            {
                Console.WriteLine("Not in Development environment, skipping user secrets");
            }

            builder.Configuration.AddEnvironmentVariables();

            // Debug: Check if connection string is loaded
            var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
            Console.WriteLine($"Connection String loaded: {!string.IsNullOrEmpty(connStr)}");
            if (!string.IsNullOrEmpty(connStr))
            {
                Console.WriteLine($"Connection String: {connStr.Substring(0, Math.Min(50, connStr.Length))}...");
            }
            
            // Try getting directly
            var directConn = builder.Configuration["ConnectionStrings:DefaultConnection"];
            Console.WriteLine($"Direct ConnectionStrings:DefaultConnection: {!string.IsNullOrEmpty(directConn)}");
            if (!string.IsNullOrEmpty(directConn))
            {
                Console.WriteLine($"Direct: {directConn.Substring(0, Math.Min(50, directConn.Length))}...");
            }
            
            // Check Jwt
            var jwtSecret = builder.Configuration["Jwt:SecretKey"];
            Console.WriteLine($"Jwt:SecretKey loaded: {!string.IsNullOrEmpty(jwtSecret)}");

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddApplication();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            // Authorization policies: we will add a fallback policy registration point here.
            builder.Services.AddAuthorization(options =>
            {
                // Example: dynamic policies can be added later; keep default policy strict
            });

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDBcontext>();
                await DatabaseSeeder.SeedAsync(context);
                Console.WriteLine("DONE DB");
            }

            // Configure the HTTP request pipeline.
            app.MapOpenApi();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();
            app.MapHub<infrastructure.Hubs.NotificationHub>("/hubs/notifications");

            app.Run();
        }
    }
}
