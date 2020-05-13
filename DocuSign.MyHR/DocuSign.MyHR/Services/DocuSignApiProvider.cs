using System;
using System.Linq;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace DocuSign.MyHR.Services
{
    public class DocuSignApiProvider : IDocuSignApiProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private Lazy<IUsersApi> _usersApi => new Lazy<IUsersApi>(() => new UsersApi(_docuSignConfig.Value));
        private Lazy<IEnvelopesApi> _envelopApi => new Lazy<IEnvelopesApi>(() => new EnvelopesApi(_docuSignConfig.Value));

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
                return docuSignConfig;
            });

        public DocuSignApiProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IUsersApi UsersApi => _usersApi.Value;
        public IEnvelopesApi EnvelopApi => _envelopApi.Value;
    }
}
