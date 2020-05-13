using System; 
namespace DocuSign.MyHR.Domain
{
    public class DocuSignToken
    {
        public string UserId { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? ExpireIn { get; set; }
    }
}
 