using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyWebApp.Services;

namespace MyWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Create and run the application
            var builder = WebApplication.CreateBuilder(args);

            // Register services
            builder.Services.AddControllers();
            builder.Services.AddSingleton<AttackService>(); // Register AttackService

            // Configure CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalhost", policy =>
                {
                    policy.WithOrigins("http://localhost:5173") // Allow your frontend's URL
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });

            var app = builder.Build();

            // Configure middleware pipeline
            app.UseCors("AllowLocalhost"); // Apply CORS policy

            // Redirect HTTP to HTTPS but exclude preflight (OPTIONS) requests
            app.Use((context, next) =>
            {
                if (!context.Request.IsHttps && context.Request.Method != "OPTIONS")
                {
                    context.Response.Redirect("https://localhost:5240" + context.Request.Path);
                    return Task.CompletedTask;
                }
                return next();
            });

            // Use HTTPS redirection only for non-preflight requests
            app.UseHttpsRedirection();

            app.UseRouting();
            app.MapControllers(); // Maps controllers to routes

            // Run the application
            app.Run();
        }
    }
}
