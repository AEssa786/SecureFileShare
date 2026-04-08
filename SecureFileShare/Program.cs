using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SecureFileShare.Data;
using SecureFileShare.Models;

namespace SecureFileShare
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<SecureFileShareContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<SecureFileShareContext>();

            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();
            builder.Services.AddSignalR();
            builder.Services.AddSession();

            var app = builder.Build();


            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseSession();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.MapRazorPages();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapHub<Data.SignalR.chatHub>("/chatHub");

            app.Run();
        }
    }

}
