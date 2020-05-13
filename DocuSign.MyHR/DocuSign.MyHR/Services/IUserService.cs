using DocuSign.MyHR.Domain;

namespace DocuSign.MyHR.Services
{
    public interface IUserService
    {
        UserDetails GetUserDetails(string accountId, string userId);
        UserDetails UpdateUserDetails(string accountId, string userId, UserDetails userDetails);
    }
}