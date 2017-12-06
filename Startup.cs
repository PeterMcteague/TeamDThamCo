using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using BasketService.Models;
using Microsoft.EntityFrameworkCore;
using BasketService.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using BasketService.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;

namespace BasketService
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
            // Swagger API help page UI
            services.AddMvc();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "Basket Service API",
                    Version = "v1",
                    Description = "A ASP.NET Core Web API for the Basket Service. Make sure that you press the authorize button and ender a valid bearer token for access. " +
                    "Valid bearer tokens can be found on the auth0 dashboard.",
                    TermsOfService = "For between service communications",
                });
                c.AddSecurityDefinition("JWT Token", new ApiKeyScheme
                {
                    Description = "JWT Token",
                    Name = "Authorization",
                    In = "header"
                });
            });

            // Database
#if DEBUG
            services.AddDbContext<BasketContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("BasketContext")));
#else
            services.AddDbContext<BasketContext>(opt => opt.UseMySql(Configuration.GetConnectionString("BasketContext")));
#endif
            // Auth0 authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = Configuration["Auth0:Authority"];
                options.Audience = Configuration["Auth0:Audience"];
            });

            // register config 
            services.AddSingleton<IConfiguration>(Configuration);

            // register the scope authorization handler
            services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable static files so we can return something else if user isn't authenticated
            app.UseStaticFiles();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Basket Service API");
            });
            
            // Enable authentication
            app.UseAuthentication();

            // Set up MVC routing
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
