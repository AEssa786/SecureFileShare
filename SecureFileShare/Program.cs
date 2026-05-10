using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SecureFileShare.Data;
using SecureFileShare.Data.RepositoryPattern;
using SecureFileShare.Models;
using SecureFileShare.Services;

namespace SecureFileShare
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("SecureFileShareContextConnection") ?? throw new InvalidOperationException("Connection string 'SecureFileShareContextConnection' not found.");
            builder.Services.AddDbContext<SecureFileShareContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<SecureFileShareContext>();

            builder.Services.AddScoped<IFileRepository, FileRepo>();
            builder.Services.AddScoped<IChatRepository, ChatRepo>();
            builder.Services.AddScoped<IUserRepository, UserRepo>();
            builder.Services.AddScoped<IFileService, FileService>();

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

            using (var scope = app.Services.CreateScope())
            {
                var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                string[] roles = { "Admin", "User" };
                foreach (var role in roles)
                {
                    if (!await roleMgr.RoleExistsAsync(role))
                    {
                        await roleMgr.CreateAsync(new IdentityRole(role));
                    }
                }
            }

            app.Run();
        }
    }

}
