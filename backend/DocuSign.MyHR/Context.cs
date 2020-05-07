using System.Security.Claims;
using DocuSign.MyHR.Domain;
using Microsoft.Extensions.Caching.Memory;

namespace DocuSign.MyHR
{
    public class Context
    {
        private readonly IMemoryCache _cache; 

        public Context(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void Init(ClaimsPrincipal principalUser)
        {
            User = _cache.Get<User>(GetKey(principalUser.FindFirstValue(ClaimTypes.NameIdentifier), "User"));
        }

        public void SetUser(ClaimsPrincipal principalUser)
        {
            var userId = principalUser.FindFirstValue(ClaimTypes.NameIdentifier);
            User = new User
            {
                Id = userId,
                Name = principalUser.FindFirstValue(ClaimTypes.Name),
            };
            _cache.Set(GetKey(userId, "User"), User);
        }

        public static User User { get; private set; }

        private string GetKey(string id, string key)
        {
            return $"{id}_{key}";
        }
    }
}
