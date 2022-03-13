using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PersonGeneratorApi.Middleware;


namespace PersonGeneratorApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddConfiguration(configuration);
            builder.AddJsonFile("appsettings.json", optional: true);
        }

        private void FoldersCreate()
        {
            Directory.CreateDirectory("Images/Identified");
            Directory.CreateDirectory("Images/Unidentified");
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<KeyValidationMiddleware>();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            FoldersCreate();
        }
    }
}
