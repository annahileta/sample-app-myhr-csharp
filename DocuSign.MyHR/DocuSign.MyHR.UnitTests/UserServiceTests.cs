using System;
using System.IO;
using System.Text;
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
        private static string _accountId = "1";
        private static string _userId = "2";

        [Theory, AutoData]
        public void GetUserDetails_WhenCorrectRequestParameters_ReturnsCorrectResult(UserInformation userInformation)
        {
            //Arrange
            var docuSignApiProvider = new Mock<IDocuSignApiProvider>();
            var usersApi = new Mock<IUsersApi>();
            userInformation.CreatedDateTime = DateTime.Now.ToString();
            usersApi.Setup(x => x.GetInformation(_accountId, _userId, It.IsAny<UsersApi.GetInformationOptions>())).Returns(userInformation);
            docuSignApiProvider.SetupGet(c => c.UsersApi).Returns(usersApi.Object);

            var sut = new UserService(docuSignApiProvider.Object);

            //Act
            var userDetails = sut.GetUserDetails(_accountId, _userId);

            //Assert
            Assert.NotNull(userDetails);
             
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
        public void GetUserDetails_WhenUserHasImage_ReturnsCorrectResult(UserInformation userInformation)
        {
            //Arrange
            var docuSignApiProvider = new Mock<IDocuSignApiProvider>();
            var usersApi = new Mock<IUsersApi>();
            userInformation.CreatedDateTime = DateTime.Now.ToString(); 
            usersApi.Setup(x => x.GetInformation(_accountId, _userId, It.IsAny<UsersApi.GetInformationOptions>())).Returns(userInformation);
            usersApi.Setup(x => x.GetProfileImage(_accountId, _userId, It.IsAny<UsersApi.GetProfileImageOptions>()))
                .Returns(new MemoryStream(Encoding.UTF8.GetBytes("someimage")));
            docuSignApiProvider.SetupGet(c => c.UsersApi).Returns(usersApi.Object);

            var sut = new UserService(docuSignApiProvider.Object);

            //Act
            var userDetails = sut.GetUserDetails(_accountId, _userId);

            //Assert
            Assert.NotNull(userDetails); 
            Assert.NotNull(userDetails.ProfileImage);
        }

        [Fact]
        public void GetUserDetails_WhenAccountIdNull_ThrowsArgumentException()
        {
            //Arrange
            var docuSignApiProvider = new Mock<IDocuSignApiProvider>();
            var sut = new UserService(docuSignApiProvider.Object);

            //Act 
            //Assert
            Assert.Throws<ArgumentNullException>(() => sut.GetUserDetails(null, _userId));
        }

        [Fact]
        public void GetUserDetails_WhenUserIdNull_ThrowsArgumentException()
        {
            //Arrange
            var docuSignApiProvider = new Mock<IDocuSignApiProvider>();
            var sut = new UserService(docuSignApiProvider.Object);

            //Act 
            //Assert
            Assert.Throws<ArgumentNullException>(() => sut.GetUserDetails(_userId, null));
        }

        [Theory, AutoData]
        public void UpdateUserDetails_WhenCorrectRequestParameters_CallsUpdateUserApiCorrectly(UserDetails newUserDetails)
        {
            //Arrange
            var docuSignApiProvider = new Mock<IDocuSignApiProvider>();
            var usersApi = new Mock<IUsersApi>();
            var updatedUserInfo = Convert(newUserDetails);
            usersApi.Setup(x => x.UpdateUser(_accountId, _userId, It.IsAny<UserInformation>())).Returns(updatedUserInfo);
            docuSignApiProvider.SetupGet(c => c.UsersApi).Returns(usersApi.Object);

            var sut = new UserService(docuSignApiProvider.Object);

            //Act
            sut.UpdateUserDetails(_accountId, _userId, newUserDetails);

            //Assert
            usersApi.Verify(mock => mock.UpdateUser(_accountId, _userId, updatedUserInfo), Times.Once());
        }

        [Theory, AutoData]
        public void UpdateUserDetails_WhenAccountIdNull_ThrowsArgumentException(UserDetails newUserDetails)
        {
            //Arrange
            var docuSignApiProvider = new Mock<IDocuSignApiProvider>();
            var sut = new UserService(docuSignApiProvider.Object);

            //Act 
            //Assert
            Assert.Throws<ArgumentNullException>(() => sut.UpdateUserDetails(null, _userId, newUserDetails));
        }

        [Theory, AutoData]
        public void UpdateUserDetails_WhenUserIdNull_ThrowsArgumentException(UserDetails newUserDetails)
        {
            //Arrange
            var docuSignApiProvider = new Mock<IDocuSignApiProvider>();
            var sut = new UserService(docuSignApiProvider.Object);

            //Act 
            //Assert
            Assert.Throws<ArgumentNullException>(() => sut.UpdateUserDetails(_accountId, null, newUserDetails));
        }

        [Fact]
        public void UpdateUserDetails_WhenUserDetailsNull_ThrowsArgumentException()
        {
            //Arrange
            var docuSignApiProvider = new Mock<IDocuSignApiProvider>();
            var sut = new UserService(docuSignApiProvider.Object);

            //Act 
            //Assert
            Assert.Throws<ArgumentNullException>(() => sut.UpdateUserDetails(_accountId, _userId, null));
        }

        private UserInformation Convert(UserDetails userDetails)
        {
            return new UserInformation(
                LastName: userDetails.LastName,
                FirstName: userDetails.FirstName,
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
