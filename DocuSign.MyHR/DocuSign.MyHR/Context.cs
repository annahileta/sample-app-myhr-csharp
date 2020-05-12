using System.Linq;
using System.Security.Claims;
using DocuSign.MyHR.Domain;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

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
            var userId = principalUser.FindFirstValue(ClaimTypes.NameIdentifier);
            User = new User(
                userId,
                principalUser.FindFirstValue(ClaimTypes.Name)
            );
            Account = principalUser.FindAll("accounts").Select(x => JsonConvert.DeserializeObject<Account>(x.Value))
                .First(x => x.Id == principalUser.FindFirstValue("account_id")); 
        }

        public static User User { get; private set; }
        public static Account Account { get; private set; }  
    }
}
