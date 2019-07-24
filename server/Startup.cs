using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

namespace Eze.Quantbox
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            //services.AddOpenApiDocument();
            services.AddOpenApiDocument(document =>
            {
                document.PostProcess = doc =>
                {
                    doc.Info.Title = "Quant Box API";
                    doc.Info.Description = "Interact with the Quant Box API using the methods and objects described here";
                };
            });
            services.AddCors(options => options.AddPolicy("CorsPolicy", builder =>
            {
                builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .AllowAnyOrigin()
                .WithOrigins("http://localhost:8080");
            }));
            services.AddSignalR();
            services.AddSingleton(new AlgoMaster());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseCors("CorsPolicy");
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseOpenApi();
            app.UseSwaggerUi3(); // serve Swagger UI
            //app.UseReDoc(); // serve ReDoc UI

            app.UseSignalR(routes =>
            {
                routes.MapHub<MasterHub>("/MasterHub");
            });
            app.UseMvcWithDefaultRoute();

            System.Console.WriteLine("API docs at: /swagger/");
        }
    }
}
