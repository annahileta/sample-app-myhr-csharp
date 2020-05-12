using System;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace DocuSign.MyHR.Services
{
    public class DocuSignApiProvider : IDocuSignApiProvider
    {
        private readonly Configuration _docuSignConfig;
        private Lazy<IUsersApi> _usersApi => new Lazy<IUsersApi>(() => new UsersApi(_docuSignConfig));

        public DocuSignApiProvider(IHttpContextAccessor httpContextAccessor)
        {
            var apiClient = new ApiClient(Context.Account.BaseUri + "/restapi");
            _docuSignConfig = new Configuration(apiClient);
            _docuSignConfig.AddDefaultHeader("Authorization", "Bearer " + httpContextAccessor.HttpContext.GetTokenAsync("access_token").Result);
        } 

        public IUsersApi UsersApi => _usersApi.Value;
    }
}
