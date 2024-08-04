using Azure;
using Azure.AI.TextAnalytics;
using ChatApp.Abstractions;
using ChatApp.Data;
using ChatApp.Data.Repositories;
using ChatApp.Hubs;
using ChatApp.Services;
using Microsoft.EntityFrameworkCore;

namespace ChatApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            IConfiguration configuration = builder.Configuration;

            // Add services to the container.
            builder.Services.AddDbContext<ChatContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddSignalR(); builder.Services.AddSingleton<TextAnalyticsClient>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var endpoint = configuration["AzureTextAnalytics:Endpoint"];
                var apiKey = configuration["AzureTextAnalytics:ApiKey"];
                return new TextAnalyticsClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
            });
            builder.Services.AddScoped<IChatService, ChatService>();
            builder.Services.AddScoped<IChatRepository, ChatRepository>();

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.MapHub<ChatHub>("/chat");

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");


            app.Run();
        }
    }
}
