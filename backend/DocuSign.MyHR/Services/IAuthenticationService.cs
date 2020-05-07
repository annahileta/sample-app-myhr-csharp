using System.IdentityModel.Tokens.Jwt;
namespace DocuSign.MyHR.Services
{
    public interface IAuthenticationService
    {
        string GetAuthorizationUrl(string redirectUrl);

        JwtSecurityToken Authenticate(string authCode);
       
        JwtSecurityToken AuthenticateFromJwt();
    }
}