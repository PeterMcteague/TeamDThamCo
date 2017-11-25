using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.EntityFrameworkCore;
using OrderService.Models;
using OrderService.Data;
using Stripe;
using Hangfire;
using Hangfire.SqlServer;

namespace OrderService
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public static string _stripeAPIKey { get; private set; }

        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .SetBasePath(env.ContentRootPath);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "Order Service API",
                    Version = "v1",
                    Description = "A ASP.NET Core Web API for the Order Service",
                    TermsOfService = "For between service communications",
                });
            });
            
            services.AddDbContext<OrderServiceContext>(options => options.UseSqlServer(Configuration.GetConnectionString("OrderService")));
            services.AddDbContext<HangfireContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Hangfire")));
            services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString("Hangfire")));
            _stripeAPIKey = Configuration["StripeAPIKey"];
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order Service API");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }

            // Hangfire
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<HangfireContext>();
                context.Database.Migrate();
            }
            app.UseHangfireDashboard();
            app.UseHangfireServer();
            
            app.UseMvc();
        }
    }
}
