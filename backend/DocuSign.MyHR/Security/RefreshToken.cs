using System; 

namespace DocuSign.MyHR.Security
{
    public class RefreshToken
    {
        public RefreshToken(string userId, string token, DateTime expiration)
        {
            UserId = userId;
            Token = token;
            Expiration = expiration; 
        }

        public string UserId { get;  } 
        public string Token { get; } 
        public DateTime Expiration { get; } 
    }
}