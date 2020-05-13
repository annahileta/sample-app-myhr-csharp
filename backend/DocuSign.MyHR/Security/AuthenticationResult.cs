using System.IdentityModel.Tokens.Jwt;
using DocuSign.eSign.Client.Auth;

namespace DocuSign.MyHR.Security
{
    public class AuthenticationResult
    {
        public AuthenticationResult(JwtSecurityToken accessToken, RefreshToken refreshToken, OAuth.UserInfo userInfo)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            UserInfo = userInfo;
        }

        public JwtSecurityToken AccessToken { get;  }
        public RefreshToken RefreshToken { get;  }
        public OAuth.UserInfo UserInfo { get;  }
    }
}
