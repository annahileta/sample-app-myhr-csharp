
// TODO: add following lines to your Startup.cs:
// 1. ConfigureMVCForDS to 'ConfigureServices' method, for example: services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1).ConfigureMVCForDS();
// 2. ConfigureDS to 'ConfigureServices' method, for example: services.ConfigureDS(Configuration);
// 3. app.ConfigureDS(); to 'Configure' method, for example: app.ConfigureDS();

using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace DocuSign.MyHR.DocuSign.eSignature
{

    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authentication.OAuth;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public static class DocuSignWebAppExtensions
    {
        public static void ConfigureDS(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseAuthentication();
            applicationBuilder.UseSession();
        }

        public static void ConfigureDS(this IServiceCollection services, IConfiguration Configuration)
        {
            DsConfiguration config = new DsConfiguration();

            Configuration.Bind("DocuSign", config);
            services.AddSingleton(config);
            services.AddScoped<IRequestItemsService, RequestItemsService>();

            services.AddMemoryCache();
            services.AddSession();
            services.AddHttpContextAccessor();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddAuthentication(options =>
            {
                options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "DocuSign";
            })
            .AddOAuth("DocuSign", options =>
            {
                options.ClientId = Configuration["DocuSign:IntegrationKey"];
                options.ClientSecret = Configuration["DocuSign:SecretKey"];
                options.CallbackPath = new PathString("/ds/callback");

                options.AuthorizationEndpoint = Configuration["DocuSign:AuthorizationEndpoint"];
                options.TokenEndpoint = Configuration["DocuSign:TokenEndpoint"];
                options.UserInformationEndpoint = Configuration["DocuSign:UserInformationEndpoint"];
                options.Scope.Add("signature");
                options.SaveTokens = true;
                options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "sub");
                options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
                options.ClaimActions.MapJsonKey("accounts", "accounts");
                options.ClaimActions.MapCustomJson("account_id", obj => ExtractDefaultAccountValue(obj, "account_id"));
                options.ClaimActions.MapJsonKey("access_token", "access_token");
                options.ClaimActions.MapJsonKey("refresh_token", "refresh_token");
                options.ClaimActions.MapJsonKey("expires_in", "expires_in");
                options.Events = new OAuthEvents
                {
                    OnCreatingTicket = async context =>
                    {
                        var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

                        var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                        response.EnsureSuccessStatusCode();
                        var user = JObject.Parse(await response.Content.ReadAsStringAsync());

                        user.Add("access_token", context.AccessToken);
                        user.Add("refresh_token", context.RefreshToken);
                        user.Add("expires_in", DateTime.Now.Add(context.ExpiresIn.Value).ToString());

                        using (JsonDocument payload = JsonDocument.Parse(user.ToString()))
                        {
                            context.RunClaimActions(payload.RootElement);
                        }
                    },
                    OnRemoteFailure = context =>
                    {
                        context.HandleResponse();
                        context.Response.Redirect("/Error?message=" + context.Failure.Message);
                        return Task.FromResult(0);
                    },
                    OnRedirectToAuthorizationEndpoint = context =>
                    {
                        if (context.Request.Path.StartsWithSegments("/api"))
                        {
                            context.Response.Headers["Location"] = context.RedirectUri;
                            context.Response.StatusCode = 401;
                        }
                        else
                        {
                            context.Response.Redirect(context.RedirectUri);
                        } 

                        return Task.CompletedTask;
                    }
                };
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, config =>
            {
                config.Cookie.Name = "UserLoginCookie";
                config.LoginPath = "/Account/Login?authType=JWT";
            });
            services.AddAuthorization(options =>
            {
                var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
                    "DocuSign",
                    CookieAuthenticationDefaults.AuthenticationScheme);
                defaultAuthorizationPolicyBuilder = defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
                options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
            });

        }
        private static string ExtractDefaultAccountValue(JsonElement obj, string key)
        {
            if (!obj.TryGetProperty("accounts", out var accounts))
            {
                return null;
            }

            string keyValue = null;

            foreach (var account in accounts.EnumerateArray())
            {
                if (account.TryGetProperty("is_default", out var defaultAccount) && defaultAccount.GetBoolean())
                {
                    if (account.TryGetProperty(key, out var value))
                    {
                        keyValue = value.GetString();
                    }
                }
            }

            return keyValue;
        }
    }
}

namespace DocuSign.MyHR.DocuSign.eSignature
{
    using System;

    public class Locals
    {
        public DsConfiguration DsConfig { get; set; }
        public User User { get; set; }
        public Session Session { get; set; }
        public String Messages { get; set; }
        public object Json { get; internal set; }
    }

    public class DsConfiguration
    {
        public string AppUrl { get; set; }

        public string SignerEmail { get; set; }

        public string SignerName { get; set; }

        public string GatewayAccountId { get; set; }

        public string GatewayName { get; set; }

        public string GatewayDisplayName { get; set; }

        public bool production = false;
        public bool debug = true; // Send debugging statements to console
        public string sessionSecret = "12345"; // Secret for encrypting session cookie content
        public bool allowSilentAuthentication = true; // a user can be silently authenticated if they have an
                                                      // active login session on another tab of the same browser
                                                      // Set if you want a specific DocuSign AccountId, If null, the user's default account will be used.
        public string targetAccountId = null;
        public string demoDocPath = "demo_documents";
        public string docDocx = "World_Wide_Corp_Battle_Plan_Trafalgar.docx";
        public string tabsDocx = "World_Wide_Corp_salary.docx";
        public string docPdf = "World_Wide_Corp_lorem.pdf";
        public string githubExampleUrl = "https://github.com/docusign/eg-03-csharp-auth-code-grant-core/tree/master/eg-03-csharp-auth-code-grant-core/Controllers/";
        public string documentation = null;

        public static DsConfiguration Instance { get; private set; } = new DsConfiguration();
    }

    public class Session
    {
        public string AccountId { get; set; }
        public string AccountName { get; set; }
        public string BasePath { get; set; }
    }

    public class User
    {
        public string Name { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? ExpireIn { get; set; }
    }

    public interface IRequestItemsService
    {
        Session Session { get; set; }

        User User { get; set; }
    }
}
