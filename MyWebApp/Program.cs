using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyWebApp.Services;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace MyWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ✅ Flip this manually depending on where you're running
            bool isLocal = builder.Environment.IsDevelopment();

            // Register services
            builder.Services.AddControllers();
            builder.Services.AddSingleton<AttackService>();

            // ✅ CORS policies
            builder.Services.AddCors(options =>
            {
                if (isLocal)
                {
                    options.AddPolicy("AllowLocalhost", policy =>
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
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });
                }
            });

            builder.Services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = 429;

                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                {
                    var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                    var path = httpContext.Request.Path.Value ?? "unknown";

                    // 👇 Composite key: "IP|Path"
                    var key = $"{ip}|{path}";

                    return RateLimitPartition.GetFixedWindowLimiter(key, _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 3,
                        Window = TimeSpan.FromSeconds(1),
                        QueueLimit = 0,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                    });
                });
            });

            var app = builder.Build();

            // ✅ Use CORS policy depending on mode
            app.UseCors(isLocal ? "AllowLocalhost" : "AllowAll");

            // ✅ HTTPS redirect only in local dev
            if (isLocal)
            {
                app.Use(async (context, next) =>
                {
                    if (!context.Request.IsHttps && context.Request.Method != "OPTIONS")
                    {
                        context.Response.Redirect("https://localhost:5240" + context.Request.Path);
                        return;
                    }
                    await next();
                });

                app.UseHttpsRedirection();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseRateLimiter();
            app.MapControllers();
            app.MapFallbackToFile("index.html");

            app.Run();
        }
    }
}
