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
        private readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddConfiguration(configuration);
            builder.AddJsonFile("appsettings.json", optional: true);
            _configuration = configuration;
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

            PersonGenerator.PersonGeneratorConfigure(_configuration);
            CreateFolders();
        }

        private void CreateFolders()
        {
            Directory.CreateDirectory(_configuration["UnidentPath"]);
            Directory.CreateDirectory(_configuration["IdentPath"]);
        }
    }
}
