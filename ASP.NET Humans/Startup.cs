using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ASP.NET_Humans.Domain;
using ASP.NET_Humans.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Routing;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace ASP.NET_Humans
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.Configure<AnabolkickCompany>(Configuration.GetSection("AnabolkickCompany"));

            services.AddDbContext<AppDbContext>(x =>
                x.UseSqlServer(Configuration.GetConnectionString("LocalDatabase")));

            services.AddIdentity<User, IdentityRole>(options =>
                {
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredUniqueChars = 2;
                    options.Password.RequireNonAlphanumeric = false;
                    options.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

        }

        private void CreateRoles(RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            if (!roleManager.RoleExistsAsync("Admin").Result)
            {
                var role = new IdentityRole("Admin");
                Task.Run(() => roleManager.CreateAsync(role)).Wait();

                //Create admin
                var user = new User { UserName = "Admin", Email = "oleg10galysh@gmail.com", EmailConfirmed = true };
                string pass = "Anab@1kick";

                Task.Run(() => userManager.CreateAsync(user, pass)).Wait();
                if (userManager.CreateAsync(user, pass).Result.Succeeded)
                {
                    Task.Run(() => userManager.AddToRoleAsync(user, "Admin")).Wait();
                }
            }

            if (!roleManager.RoleExistsAsync("User").Result)
            {
                var role = new IdentityRole() { Name = "User" };
                Task.Run(() => roleManager.CreateAsync(role)).Wait();
            }
        }

        private void FoldersCreate()
        {
            Directory.CreateDirectory("wwwroot/Images/Users");
        }
        private void DeleteRole(RoleManager<IdentityRole> roleManager)
        {
            var role = roleManager.FindByNameAsync("User").Result;
            var result = roleManager.DeleteAsync(role);
            Console.WriteLine(result.Result.Errors);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 404)
                {
                    context.Response.Redirect("/Error/Error_404");
                }
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id:int?}");
            });

            CreateRoles(roleManager, userManager);
            FoldersCreate();
        }
    }
}
