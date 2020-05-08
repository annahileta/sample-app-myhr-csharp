namespace DocuSign.MyHR.Security
{
    public interface IAuthenticationService
    {
        string GetAuthorizationUrl(string redirectUrl);
        AuthenticationResult Authenticate(string authCode);
        AuthenticationResult AuthenticateFromJwt();
        AuthenticationResult RefreshToken(string refreshToken);
        void Logout(string userId);
    }
}