using System;
using System.Text;
using DocuSign.MyHR.Repositories;
using DocuSign.MyHR.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace DocuSign.MyHR
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
            services.AddControllers();
            services.AddMemoryCache(); 
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<ITokenRepository, TokenRepository>();
            services.AddSingleton<Context, Context>();
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddMvc(options => options.Filters.Add(typeof(ContextFilter)));
            services
                .AddAuthentication(o =>
                       {
                           o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                           o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                       })
                .AddJwtBearer(cfg =>
                  {
                      cfg.RequireHttpsMetadata = false;
                      cfg.SaveToken = true; 
                      cfg.TokenValidationParameters = new TokenValidationParameters()
                      {
                          ValidateIssuerSigningKey = true,
                          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtSecretKey"])),
                          ValidateIssuer = false,
                          ValidateAudience = false
                      };
                  });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting(); 
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}");
                endpoints.MapControllerRoute("api", "api/{controller=Home}/{action=Index}");
            });
        }
    }
}
