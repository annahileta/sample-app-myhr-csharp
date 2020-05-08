using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text; 
using DocuSign.eSign.Client.Auth;
using Microsoft.IdentityModel.Tokens;

namespace DocuSign.MyHR.Security
{
    public class TokenProvider
    { 
        public static JwtSecurityToken GenerateJwtAccessToken(OAuth.UserInfo userInfo, string secretKey)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.Sub),
                new Claim(JwtRegisteredClaimNames.UniqueName, userInfo.Name),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwtToken = new JwtSecurityToken(String.Empty,
                String.Empty,
                claims,
                expires: DateTime.Now.AddSeconds(55 * 60),
                signingCredentials: creds);
            return jwtToken;
        }

        public static RefreshToken GenerateRefreshToken(OAuth.UserInfo userInfo)
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return new RefreshToken(
                    userInfo.Sub, 
                    Convert.ToBase64String(randomNumber), 
                    DateTime.Now.AddDays(1));
        }

        public static void ValidateRefreshToken(string refreshToken, RefreshToken storedRefreshToken)
        {
            if (refreshToken != storedRefreshToken.Token)
            {
                throw new SecurityTokenException("Token invalid.");
            }

            // Ensure that the refresh token that we got from storage is not yet expired.
            if (DateTime.UtcNow > storedRefreshToken.Expiration)
            {
                throw new SecurityTokenException("Token invalid.");
            }
        }
    }
}
