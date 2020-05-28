using System;
using System.Collections.Generic;
using System.Security.Claims;
using Moq;
using Xunit;
using AutoFixture.Xunit2;
using DocuSign.MyHR.Controllers;
using DocuSign.MyHR.Domain;
using DocuSign.MyHR.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DocuSign.MyHR.UnitTests
{
    public class UserControllerTests
    {
        [Theory, AutoData]
        public void Index_Get_ReturnsCorrectResult(
            Mock<IUserService> userService,
            UserDetails userDetails,
            Account account,
            User user)
        {
            InitContext(account, user);
            userService.Setup(c => c.GetUserDetails(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(userDetails);

            var sut = new UserController(userService.Object);

            var result = sut.Index();
            Assert.True(result is OkObjectResult); 
            Assert.IsType<UserDetails>(((OkObjectResult)result).Value);
            var receivedUser = (UserDetails) ((OkObjectResult) result).Value;
            Assert.Equal(userDetails.Address, receivedUser.Address);
            Assert.Equal(userDetails.Email, receivedUser.Email);
            Assert.Equal(userDetails.Id, receivedUser.Id);
        }

        [Theory, AutoData]
        public void Index_Put_ReturnsCorrectResult(
            Mock<IUserService> userService,
            UserDetails userDetails,
            Account account,
            User user)
        {
            InitContext(account, user);

            var sut = new UserController(userService.Object);

            var result = sut.Index(userDetails);
            Assert.True(result is OkResult);
        }
         
        private void InitContext(Account account, User user)
        {
            var context = new Context();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim("authType", LoginType.CodeGrant.ToString())
            };
            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            claimsIdentity.AddClaim(new Claim("accounts", JsonConvert.SerializeObject(account)));
            claimsIdentity.AddClaim(new Claim("account_id", account.Id));

            context.Init(new ClaimsPrincipal(claimsIdentity));
        }
    }
}
