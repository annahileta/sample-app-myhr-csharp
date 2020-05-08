using Microsoft.Extensions.Caching.Memory;

namespace DocuSign.MyHR.DocuSign.eSignature
{
    public class RequestItemsService : IRequestItemsService
    {
        private readonly IMemoryCache _cache;
        private string _id;
        private string _accessToken;

        public RequestItemsService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void UpdateValues(User user, Session session, string id)
        {
            _id = id;
            _accessToken = user.AccessToken;
            User = user;
            Session = session;
        }

        private string GetKey(string key)
        {
            return string.Format("{0}_{1}", _id, key);
        }

        public Session Session
        {
            get => _cache.Get<Session>(GetKey("Session"));
            set => _cache.Set(GetKey("Session"), value);
        }

        public User User
        {
            get => _cache.Get<User>(GetKey("User"));
            set => _cache.Set(GetKey("User"), value);
        }
    }
}