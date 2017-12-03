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
using Hangfire.MySql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using OrderService.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace OrderService
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
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
                c.OperationFilter<AddAuthTokenHeader>();
            });

            // Add databases
#if DEBUG
            services.AddDbContext<OrderServiceContext>(options => options.UseSqlServer(Configuration.GetConnectionString("OrderService")));
            services.AddDbContext<HangfireContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Hangfire")));
            services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString("Hangfire")));
#else
            services.AddDbContext<OrderServiceContext>(options => options.UseMySql(Configuration.GetConnectionString("OrderService")));
            services.AddDbContext<HangfireContext>(options => options.UseMySql(Configuration.GetConnectionString("Hangfire")));
            GlobalConfiguration.Configuration.UseStorage(new MySqlStorage("Hangfire"));
#endif
            // Configure stripe payment API key
            StripeConfiguration.SetApiKey(Configuration.GetSection("Keys").GetValue<string>("Stripe"));


            // Setup Auth0 authentication
            string domain = $"https://{Configuration["Auth0:Domain"]}/";
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.Authority = domain;
                options.Audience = Configuration["Auth0:ApiIdentifier"];
            });            

            services.AddAuthorization(options =>
            {
                options.AddPolicy("read:order", policy => policy.Requirements.Add(new HasScopeRequirement("read:order", domain)));
                options.AddPolicy("create:order", policy => policy.Requirements.Add(new HasScopeRequirement("create:order", domain)));
                options.AddPolicy("create:payment", policy => policy.Requirements.Add(new HasScopeRequirement("create:payment", domain)));
            });




            // register the scope authorization handler
            services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable static files so we can return something else if user isn't authenticated
            app.UseStaticFiles();

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
                context.SaveChanges();
            }
            app.UseHangfireDashboard();
            app.UseHangfireServer();

            // Authentication
            app.UseAuthentication();

            // MVC routine
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
