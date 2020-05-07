using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using DocuSign.eSign.Client;
using DocuSign.eSign.Client.Auth;
using DocuSign.MyHR.Domain;
using DocuSign.MyHR.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens; 

namespace DocuSign.MyHR.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ITokenRepository _tokenRepository;
        private readonly IConfiguration _configurationService;
        private ApiClient _apiClient;

        public AuthenticationService(ITokenRepository tokenRepository, IConfiguration configurationService)
        {
            _tokenRepository = tokenRepository;
            _configurationService = configurationService;
            _apiClient = new ApiClient();
            _apiClient.SetOAuthBasePath(_configurationService["DocuSign:AuthServer"]);
        }

        public string GetAuthorizationUrl(string redirectUrl)
        {
            return $"https://{_configurationService["DocuSign:AuthServer"]}/oauth/auth?&scope=signature" +
                   $"&client_id={_configurationService["DocuSign:IntegrationKey"]}" +
                   $"&redirect_uri={redirectUrl}";
        }

        public JwtSecurityToken AuthenticateFromJwt()
        {
            OAuth.OAuthToken authToken = _apiClient.RequestJWTUserToken(
                _configurationService["DocuSign:IntegrationKey"],
                _configurationService["DocuSign:UserId"],
                _configurationService["DocuSign:AuthServer"],
                Encoding.UTF8.GetBytes(_configurationService["DocuSign:RSAKey"]),
                1);

            OAuth.UserInfo userInfo = _apiClient.GetUserInfo(authToken.access_token);
            return CreateJwt(userInfo);
        }

        public JwtSecurityToken Authenticate(string authCode)
        {
            OAuth.OAuthToken token = _apiClient.GenerateAccessToken(
                _configurationService["DocuSign:IntegrationKey"],
                _configurationService["DocuSign:SecretKey"],
                authCode);

            OAuth.UserInfo userInfo = _apiClient.GetUserInfo(token.access_token);

            SaveDocuSignToken(userInfo, token);
            return CreateJwt(userInfo);
        }

        private JwtSecurityToken CreateJwt(OAuth.UserInfo userInfo)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.Sub),
                new Claim(JwtRegisteredClaimNames.UniqueName, userInfo.Name),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configurationService["JwtSecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwtToken = new JwtSecurityToken(String.Empty,
                String.Empty,
                claims,
                expires: DateTime.Now.AddSeconds(55 * 60),
                signingCredentials: creds);
            return jwtToken;
        }

        private void SaveDocuSignToken(OAuth.UserInfo userInfo, OAuth.OAuthToken token)
        { 
            var tokenInfo = new DocuSignToken
            {
                UserId = userInfo.Accounts.First().AccountId, 
                AccessToken = token.access_token,
                ExpireIn = DateTime.Now.AddSeconds(token.expires_in.Value),
                RefreshToken = token.refresh_token 
            };
            _tokenRepository.SaveToken(tokenInfo);
        }
    }
}