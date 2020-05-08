using DocuSign.MyHR.Domain;
using DocuSign.MyHR.Security;
using Microsoft.Extensions.Caching.Memory;

namespace DocuSign.MyHR.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly IMemoryCache _cache;

        public TokenRepository(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void SaveToken(DocuSignToken token)
        {
            _cache.Set(GetKey(token.UserId, "DocuSignUserTooken"), token);
        }

        public DocuSignToken GetDocuSignToken(string userId)
        {
            return _cache.Get<DocuSignToken>(GetKey(userId, "DocuSignUserTooken"));
        }

        public void RemoveTokens(string userId)
        {
            _cache.Set(GetKey(userId, "DocuSignUserTooken"), (DocuSignToken)null);
        }

        private string GetKey(string id, string key)
        {
            return $"{key}_{id}";
        }
    }
}
