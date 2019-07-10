using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System.IO;
using System.Diagnostics;

namespace Eze.AdminConsole
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
            services.AddSwaggerDocument();
            services.AddCors(options => options.AddPolicy("CorsPolicy", builder =>
            {
                builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                //    .AllowAnyOrigin();
                .WithOrigins("http://localhost:8080", "http://localhost:8081", "http://localhost:5000", "http://marston9020b.ezesoft.net:5000");
            }));
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseCors("CorsPolicy");
            app.UseDefaultFiles();
            app.UseStaticFiles();            
            app.UseSwagger();
            app.UseSwaggerUi3();
            app.UseSignalR(routes =>
            {
                routes.MapHub<ServiceMgmtHub>("/ServiceMgmtHub");
            });
            app.UseMvcWithDefaultRoute();
        }
    }
}
