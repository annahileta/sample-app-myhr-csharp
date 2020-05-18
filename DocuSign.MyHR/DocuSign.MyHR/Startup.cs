using System.Threading.Tasks;
using DocuSign.MyHR.Security;
using DocuSign.MyHR.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
            services.AddMemoryCache();
            services.AddHttpContextAccessor();
            services.AddScoped<Context, Context>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IDocuSignApiProvider, DocuSignApiProvider>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEnvelopeService, EnvelopeService>();
            services.AddHttpClient<DocuSignApiProvider>();
            services.AddScoped<IClickWrapService, ClickWrapService>();

            services.AddControllersWithViews();
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
            services.ConfigureDocuSignSSO(Configuration);
            services.AddMvc(options => options.Filters.Add(typeof(ContextFilter)))
                            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddRazorPages().AddNewtonsoftJson();
 
            services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.Headers["Location"] = context.RedirectUri;
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
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
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }
            app.UseRouting();
            app.ConfigureDocuSign();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });
            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }

        public class JsonFormatter : SystemTextJsonInputFormatter
        {
            public JsonFormatter(
                JsonOptions options,
                ILogger<SystemTextJsonInputFormatter> logger)
            : base(options, logger)
            {
            }

            public override bool CanRead(InputFormatterContext context)
            {
                return base.CanRead(context);
            }
        }
    }
}
