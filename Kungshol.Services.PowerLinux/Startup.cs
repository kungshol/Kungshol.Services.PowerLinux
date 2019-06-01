using Kungshol.Services.PowerLinux.OpenApi;
using Kungshol.Services.PowerLinux.Routes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace Kungshol.Services.PowerLinux
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
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("default",
                    new Info { Title = OpenApiConstants.ApiName, Version = OpenApiConstants.ApiVersion });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            string jsonFileName = "openapi.json";
            string docsRoutePrefix = RouteConstants.DocsRoute.TrimStart('/');

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger(c => { c.RouteTemplate = $"{docsRoutePrefix}/{{documentName}}/{jsonFileName}"; });

            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = docsRoutePrefix;
                c.SwaggerEndpoint($"/{docsRoutePrefix}/default/{jsonFileName}", OpenApiConstants.ApiName);
            });

            app.UseMvc();
        }
    }
}