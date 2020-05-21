using System;
using System.IO;
using DocuSign.eSign.Model;
using DocuSign.MyHR.Domain;
using RestSharp.Extensions;

namespace DocuSign.MyHR.Services
{
    public class UserService : IUserService
    {
        private readonly IDocuSignApiProvider _docuSignApiProvider;

        public UserService(IDocuSignApiProvider docuSignApiProvider)
        {
            _docuSignApiProvider = docuSignApiProvider;
        }

        public UserDetails GetUserDetails(string accountId, string userId)
        {
            UserInformation userInfo = _docuSignApiProvider.UsersApi.GetInformation(accountId, userId);
            Stream image = _docuSignApiProvider.UsersApi.GetProfileImage(accountId, userId);
            UserDetails userDetails = GetUserDetails(userInfo);
            userDetails.ProfileImage = Convert.ToBase64String(image.ReadAsBytes());
            return userDetails;
        }
         
        public UserDetails UpdateUserDetails(string accountId, string userId, UserDetails userDetails)
        {
            UserInformation updatedUserInfo = _docuSignApiProvider.UsersApi.UpdateUser(
                accountId, userId, new UserInformation(
                    UserName: userDetails.Name,
                    HomeAddress: new AddressInformation(
                        userDetails.Address.Address1,
                        userDetails.Address.Address2,
                        userDetails.Address.City,
                        userDetails.Address.Country,
                        userDetails.Address.Fax,
                        userDetails.Address.Phone,
                        userDetails.Address.PostalCode,
                        userDetails.Address.StateOrProvince
                        )));

            return GetUserDetails(updatedUserInfo);
        }

        private static UserDetails GetUserDetails(UserInformation userInfo)
        {
            AddressInformation address = userInfo.HomeAddress;
            return new UserDetails(
                userInfo.UserId,
                userInfo.UserName,
                userInfo.Email,
                userInfo.FirstName,
                userInfo.LastName,
                DateTime.Parse(userInfo.CreatedDateTime),
                userInfo.PermissionProfileId,
                new Address(
                    address.Address1,
                    address.Address2,
                    address.City,
                    address.Country,
                    address.Fax,
                    address.Phone,
                    address.PostalCode,
                    address.StateOrProvince));
        }
    }
}
