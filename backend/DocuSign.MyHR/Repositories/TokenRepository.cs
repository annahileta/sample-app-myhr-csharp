using DocuSign.MyHR.Domain;
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

        public DocuSignToken GetToken(string id)
        {
            return _cache.Get<DocuSignToken>(GetKey(id, "DocuSignUserTooken"));
        }

        private string GetKey(string id, string key)
        {
            return $"{id}_{key}";
        }
    }
}
