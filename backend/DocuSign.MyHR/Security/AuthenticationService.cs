using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using DocuSign.eSign.Client;
using DocuSign.eSign.Client.Auth;
using DocuSign.MyHR.Domain;
using DocuSign.MyHR.Repositories;
using DocuSign.MyHR.Security;
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

        public AuthenticationResult Authenticate(string authCode)
        {
            OAuth.OAuthToken token = _apiClient.GenerateAccessToken(
                _configurationService["DocuSign:IntegrationKey"],
                _configurationService["DocuSign:SecretKey"],
                authCode);

            OAuth.UserInfo userInfo = _apiClient.GetUserInfo(token.access_token);

            SaveDocuSignToken(userInfo, token);
            return CreateAuthenticationResult(userInfo);
        }

        public AuthenticationResult AuthenticateFromJwt()
        {
            OAuth.OAuthToken authToken = _apiClient.RequestJWTUserToken(
                _configurationService["DocuSign:IntegrationKey"],
                _configurationService["DocuSign:UserId"],
                _configurationService["DocuSign:AuthServer"],
                Encoding.UTF8.GetBytes(_configurationService["DocuSign:RSAKey"]),
                1);

            OAuth.UserInfo userInfo = _apiClient.GetUserInfo(authToken.access_token);
            return CreateAuthenticationResult(userInfo);
        } 

        public AuthenticationResult RefreshToken(string refreshToken)
        {
            //TODO:refsresh DocuSignToken

            var storedRefreshToken = _tokenRepository.GetRefreshToken(refreshToken);
            var docuSignToken = _tokenRepository.GetDocuSignToken(storedRefreshToken.UserId);
            OAuth.UserInfo userInfo;
            try
            {
                userInfo = _apiClient.GetUserInfo(docuSignToken.AccessToken);
            }
            catch (ApiException e)
            {
                if (e.ErrorCode == 401)
                {
                    //todoRefreshToken
                    userInfo = _apiClient.GetUserInfo(docuSignToken.AccessToken);
                }
                else
                {
                    throw;
                }
            }
            TokenProvider.ValidateRefreshToken(refreshToken, storedRefreshToken);

            return CreateAuthenticationResult(userInfo);
        }
         
        public void Logout(string userId)
        {
            _tokenRepository.RemoveTokens(userId);
        }

        private AuthenticationResult CreateAuthenticationResult(OAuth.UserInfo userInfo)
        {
            return new AuthenticationResult(
                TokenProvider.GenerateJwtAccessToken(userInfo, _configurationService["JwtSecretKey"]),
                TokenProvider.GenerateRefreshToken(userInfo),
                userInfo);
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