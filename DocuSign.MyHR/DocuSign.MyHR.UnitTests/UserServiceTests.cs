using Moq;
using Xunit;
using AutoFixture.Xunit2;
using DocuSign.eSign.Api;
using DocuSign.eSign.Model;
using DocuSign.MyHR.Domain;
using DocuSign.MyHR.Services;

namespace DocuSign.MyHR.UnitTests
{
    public class UserServiceTests
    {
        [Theory, AutoData]
        public void GetUserDetails_ReturnsCorrectResult(
            Mock<IDocuSignApiProvider> docuSignApiProvider,
            Mock<IUsersApi> usersApi,
            UserInformation userInformation,
            string accountId,
            string userId)
        {
            usersApi.Setup(x => x.GetInformation(accountId, userId, It.IsAny<UsersApi.GetInformationOptions>())).Returns(userInformation);
            docuSignApiProvider.SetupGet(c => c.UsersApi).Returns(usersApi.Object);

            var sut = new UserService(docuSignApiProvider.Object);

            var userDetails = sut.GetUserDetails(accountId, userId);
            Assert.NotNull(userDetails);
            Assert.Equal(userInformation.UserName, userDetails.Name);
            Assert.Equal(userInformation.HomeAddress.Address1, userDetails.Address.Address1);
            Assert.Equal(userInformation.HomeAddress.StateOrProvince, userDetails.Address.StateOrProvince);
            Assert.Equal(userInformation.HomeAddress.Address2, userDetails.Address.Address2);
            Assert.Equal(userInformation.HomeAddress.City, userDetails.Address.City);
            Assert.Equal(userInformation.HomeAddress.Country, userDetails.Address.Country);
            Assert.Equal(userInformation.HomeAddress.Fax, userDetails.Address.Fax);
            Assert.Equal(userInformation.HomeAddress.Phone, userDetails.Address.Phone);
            Assert.Equal(userInformation.HomeAddress.PostalCode, userDetails.Address.PostalCode);
        }

        [Theory, AutoData]
        public void UpdateUserDetails_ReturnsCorrectResult(
            Mock<IDocuSignApiProvider> docuSignApiProvider,
            Mock<IUsersApi> usersApi,
            UserDetails newUserDetails,
            string accountId,
            string userId)
        {
            var updatedUserInfo = Convert(newUserDetails);
            usersApi.Setup(x => x.UpdateUser(accountId, userId, It.IsAny<UserInformation>())).Returns(updatedUserInfo);
            docuSignApiProvider.SetupGet(c => c.UsersApi).Returns(usersApi.Object);

            var sut = new UserService(docuSignApiProvider.Object);

            var userDetails = sut.UpdateUserDetails(accountId, userId, newUserDetails);
            Assert.NotNull(userDetails);
            Assert.Equal(updatedUserInfo.UserName, userDetails.Name);
            Assert.Equal(updatedUserInfo.HomeAddress.Address1, userDetails.Address.Address1);
            Assert.Equal(updatedUserInfo.HomeAddress.StateOrProvince, userDetails.Address.StateOrProvince);
            Assert.Equal(updatedUserInfo.HomeAddress.Address2, userDetails.Address.Address2);
            Assert.Equal(updatedUserInfo.HomeAddress.City, userDetails.Address.City);
            Assert.Equal(updatedUserInfo.HomeAddress.Country, userDetails.Address.Country);
            Assert.Equal(updatedUserInfo.HomeAddress.Fax, userDetails.Address.Fax);
            Assert.Equal(updatedUserInfo.HomeAddress.Phone, userDetails.Address.Phone);
            Assert.Equal(updatedUserInfo.HomeAddress.PostalCode, userDetails.Address.PostalCode);
        }

        private UserInformation Convert(UserDetails userDetails)
        {
            return new UserInformation(
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
                )); 
        }
    }
}
