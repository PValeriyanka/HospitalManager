using HospitalDAO.Data;
using HospitalManager.Data;
using Humanizer.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace HospitalManager
{
    public class Program
    {
        public static void Main(string[] args)
        {            
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            ConfigurationBuilder builder = new();

            ///Установка пути к текущему каталогу
            builder.SetBasePath(Directory.GetCurrentDirectory());

            // получаем конфигурацию из файла appsettings.json
            builder.AddJsonFile("appsettings.json");

            // создаем конфигурацию
            IConfigurationRoot configuration = builder.AddUserSecrets<Program>().Build();

            SqlConnectionStringBuilder sqlConnectionStringBuilder = new(configuration.GetConnectionString("RemoteConnection"))
            {
                Password = configuration["Database:password"],
                UserID = configuration["Database:login"]
            };

            string connectionString = sqlConnectionStringBuilder.ConnectionString;

            services.AddDbContext<HospitalContext>(options => options.UseSqlServer(connectionString));
            services.AddMvc();
        }
    }
}
