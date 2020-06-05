using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace DocuSign.MyHR.Services
{
    [ExcludeFromCodeCoverage]
    public class DocuSignApiProvider : IDocuSignApiProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _clientFactory;

        private Lazy<IUsersApi> _usersApi => new Lazy<IUsersApi>(() => new UsersApi(_docuSignConfig.Value));
        private Lazy<IEnvelopesApi> _envelopApi => new Lazy<IEnvelopesApi>(() => new EnvelopesApi(_docuSignConfig.Value));
        private Lazy<ITemplatesApi> _templatesApi => new Lazy<ITemplatesApi>(() => new TemplatesApi(_docuSignConfig.Value));
        private Lazy<IAccountsApi> _accountsApi => new Lazy<IAccountsApi>(() => new AccountsApi(_docuSignConfig.Value));
        private Lazy<HttpClient> _docuSignHttpClient => new Lazy<HttpClient>(() =>
        {
            HttpClient client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _docuSignConfig.Value.AccessToken);
            client.BaseAddress = new Uri(Context.Account.BaseUri);
            client.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        });
         
        private Lazy<Configuration> _docuSignConfig => new Lazy<Configuration>(() =>
            {
                var apiClient = new ApiClient(Context.Account.BaseUri + "/restapi");
                var docuSignConfig = new Configuration(apiClient);
                var token = _httpContextAccessor.HttpContext.GetTokenAsync("access_token").Result;
                if (string.IsNullOrEmpty(token))
                {
                    token = _httpContextAccessor.HttpContext.User.FindAll("access_token").First().Value;
                }
                docuSignConfig.AddDefaultHeader("Authorization", "Bearer " + token);
                docuSignConfig.AccessToken = token;
                return docuSignConfig;
            });

        public DocuSignApiProvider(IHttpContextAccessor httpContextAccessor, IHttpClientFactory clientFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _clientFactory = clientFactory;
        }

        public IUsersApi UsersApi => _usersApi.Value;
        public IEnvelopesApi EnvelopApi => _envelopApi.Value;
        public HttpClient DocuSignHttpClient => _docuSignHttpClient.Value;
        public ITemplatesApi TemplatesApi => _templatesApi.Value;
        public IAccountsApi AccountsApi => _accountsApi.Value;
    }
}
