using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyWebApp.Services;
using System.IO;

namespace MyWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 👇 Flip this manually: true = local dev, false = production
            bool isLocal = false;

            // Register services
            builder.Services.AddControllers();
            builder.Services.AddSingleton<AttackService>();

            builder.Services.AddCors(options =>
            {
                if (isLocal)
                {
                    options.AddPolicy("LocalDev", policy =>
                    {
                        policy.WithOrigins("http://localhost:5173")
                              .AllowAnyMethod()
                              .AllowAnyHeader()
                              .AllowCredentials();
                    });
                }
                else
                {
                    options.AddPolicy("AllowAll", policy =>
                    {
                        policy.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                    });
                }
            });

            var app = builder.Build();

            if (isLocal)
            {
                app.UseCors("LocalDev");

                // Optional: local HTTPS redirect override
                app.Use(async (context, next) =>
                {
                    if (!context.Request.IsHttps && context.Request.Method != "OPTIONS")
                    {
                        context.Response.Redirect("https://localhost:5240" + context.Request.Path);
                        return;
                    }
                    await next();
                });
            }
            else
            {
                app.UseCors("AllowAll");
                app.UseHttpsRedirection();
            }

            // Serve static React app
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // React SPA fallback route
            app.Run(async (context) =>
            {
                context.Response.ContentType = "text/html";
                await context.Response.SendFileAsync(Path.Combine("wwwroot", "index.html"));
            });

            app.Run();
        }
    }
}
