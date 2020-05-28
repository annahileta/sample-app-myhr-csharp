using System;
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
            userInformation.CreatedDateTime = DateTime.Now.ToString();
            usersApi.Setup(x => x.GetInformation(accountId, userId, It.IsAny<UsersApi.GetInformationOptions>())).Returns(userInformation);
            docuSignApiProvider.SetupGet(c => c.UsersApi).Returns(usersApi.Object);

            var sut = new UserService(docuSignApiProvider.Object);

            var userDetails = sut.GetUserDetails(accountId, userId);
            Assert.NotNull(userDetails);
            Assert.Equal(userInformation.UserName, userDetails.Name);
            Assert.Equal(userInformation.WorkAddress.Address1, userDetails.Address.Address1);
            Assert.Equal(userInformation.WorkAddress.StateOrProvince, userDetails.Address.StateOrProvince);
            Assert.Equal(userInformation.WorkAddress.Address2, userDetails.Address.Address2);
            Assert.Equal(userInformation.WorkAddress.City, userDetails.Address.City);
            Assert.Equal(userInformation.WorkAddress.Country, userDetails.Address.Country);
            Assert.Equal(userInformation.WorkAddress.Fax, userDetails.Address.Fax);
            Assert.Equal(userInformation.WorkAddress.Phone, userDetails.Address.Phone);
            Assert.Equal(userInformation.WorkAddress.PostalCode, userDetails.Address.PostalCode);
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

             sut.UpdateUserDetails(accountId, userId, newUserDetails);
             usersApi.Verify(mock => mock.UpdateUser(accountId, userId, updatedUserInfo), Times.Once());
        }

        private UserInformation Convert(UserDetails userDetails)
        {
            return new UserInformation( 
                LastName:userDetails.LastName,
                FirstName:userDetails.FirstName,
                WorkAddress: new AddressInformation(
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
