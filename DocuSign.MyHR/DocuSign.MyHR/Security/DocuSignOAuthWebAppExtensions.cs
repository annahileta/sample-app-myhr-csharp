using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using DocuSign.MyHR.Domain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace DocuSign.MyHR.Security
{  
    public static class DocuSignOAuthWebAppExtensions
    {
        public static void ConfigureDocuSign(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseAuthentication();
            applicationBuilder.UseSession();
        }

        public static void ConfigureDocuSignSSO(this IServiceCollection services, IConfiguration Configuration)
        {
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
                options.Scope.Add("click.manage"); 
                options.SaveTokens = true;

                options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "sub");
                options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
                options.ClaimActions.MapJsonKey("accounts", "accounts");
                options.ClaimActions.MapJsonKey("authType", "authType");
                options.ClaimActions.MapCustomJson("account_id", obj => ExtractDefaultAccountValue(obj, "account_id"));
             
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
                        user.Add("authType", LoginType.CodeGrant.ToString());

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
                config.LoginPath = "/Account/Login";
                config.SlidingExpiration = true;
                config.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                config.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin =  context =>
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
                    },
                    // Check access token expiration and refresh if expired
                    OnValidatePrincipal = context =>
                    {
                        if (context.Properties.Items.ContainsKey(".Token.expires_at"))
                        {
                            var expire = DateTime.Parse(context.Properties.Items[".Token.expires_at"]);
                            if (expire < DateTime.Now)
                            {
                                var authProperties = context.Properties;
                                var options = context.HttpContext.RequestServices
                                    .GetRequiredService<IOptionsMonitor<OAuthOptions>>()
                                    .Get("DocuSign");

                                var pairs = new Dictionary<string, string>
                                {
                                    {"client_id", Configuration["DocuSign:IntegrationKey"]},
                                    {"client_secret", Configuration["DocuSign:SecretKey"]},
                                    {"grant_type", "refresh_token"},
                                    {"refresh_token", authProperties.GetTokenValue("refresh_token")}
                                };

                                // Request new access token
                                var content = new FormUrlEncodedContent(pairs);
                                var refreshResponse = options.Backchannel.PostAsync(options.TokenEndpoint, content,
                                    context.HttpContext.RequestAborted).Result;
                                refreshResponse.EnsureSuccessStatusCode();

                                var payload = JObject.Parse(refreshResponse.Content.ReadAsStringAsync().Result);

                                // Persist the new acess token
                                authProperties.UpdateTokenValue("access_token", payload.Value<string>("access_token"));
                                var refreshToken = payload.Value<string>("refresh_token");

                                if (!string.IsNullOrEmpty(refreshToken))
                                {
                                    authProperties.UpdateTokenValue("refresh_token", refreshToken);
                                }
                                if (int.TryParse(
                                    payload.Value<string>("expires_in"),
                                    NumberStyles.Integer,
                                    CultureInfo.InvariantCulture, out var seconds))
                                {
                                    var expiresAt = DateTimeOffset.UtcNow + TimeSpan.FromSeconds(seconds);
                                    authProperties.UpdateTokenValue(
                                        "expires_at",
                                        expiresAt.ToString("o", CultureInfo.InvariantCulture));
                                }
                                context.ShouldRenew = true;
                            }
                        }
                        return Task.FromResult(0);
                    }
                };
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

