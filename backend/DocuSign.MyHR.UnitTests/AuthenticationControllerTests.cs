using System;
using System.IdentityModel.Tokens.Jwt;
using Moq;
using Xunit;
using AutoFixture.Xunit2;
using DocuSign.MyHR.Controllers;
using DocuSign.MyHR.Services;
using Microsoft.AspNetCore.Mvc;

namespace DocuSign.MyHR.UnitTests
{
    public class AuthenticationControllerTests
    { 
        [Theory, AutoData]
        public void Login_WhenAuthCodeGrant_RedirectsToDocuSign(
            Mock<IAuthenticationService> authService,
            string redirectUrl)
        {
            authService.Setup(c => c.GetAuthorizationUrl(redirectUrl))
                .Returns("http://docusigntesturl/authentication");

            var sut = new AuthController(authService.Object);

            var result = sut.Login("CodeGrant", redirectUrl, "testurlcom");
            Assert.True(result is LocalRedirectResult);
            Assert.Equal("http://docusigntesturl/authentication", ((LocalRedirectResult)result).Url);
        }

        [Theory, AutoData]
        public void Login_WhenWrongAuthType_ReturnsBadRequest(
            Mock<IAuthenticationService> authService)
        {
            authService.Setup(c => c.GetAuthorizationUrl("/ds/callback"))
                .Returns("http://docusigntesturl/authentication");

            var sut = new AuthController(authService.Object);
            var result = sut.Login("WrongType", null, returnUrl: "testurlcom");

            Assert.True(result is BadRequestObjectResult);
        }

        [Theory, AutoData]
        public void Login_AuthCodeGrant_AndEmptyReturnUrl_RedirectsToDocuSign(
            Mock<IAuthenticationService> authService, string redirectUrl)
        {
            authService.Setup(c => c.GetAuthorizationUrl(redirectUrl))
                .Returns("http://docusigntesturl/authentication");

            var sut = new AuthController(authService.Object);

            var result = sut.Login("CodeGrant", redirectUrl, "testurlcom");
            Assert.True(result is LocalRedirectResult);
            Assert.Equal("http://docusigntesturl/authentication", ((LocalRedirectResult)result).Url);
        }

        [Theory, AutoData]
        public void Login_WhenAuthJwt_RedirectsToDocuSign(
            Mock<IAuthenticationService> authService,
            string redirectUrl, string returnUrl)
        {
            var sut = new AuthController(authService.Object);

            var result = sut.Login("JWT", redirectUrl, returnUrl);
            Assert.True(result is LocalRedirectResult);
            Assert.Equal(returnUrl, ((LocalRedirectResult)result).Url);
        }


        [Theory, AutoData]
        public void GetToken_WhenAuthenticationSuccess_OkResult(
            Mock<IAuthenticationService> authService,
            string code)
        {
            authService.Setup(c => c.Authenticate(code))
                .Returns(new JwtSecurityToken(expires:DateTime.MinValue));

            var sut = new AuthController(authService.Object);

            var result = sut.GetToken(code);
            Assert.True(result is OkObjectResult); 
        }

        [Theory, AutoData]
        public void GetToken_WhenAuthenticationFailed_BadRequest(
            Mock<IAuthenticationService> authService)
        {
            authService.Setup(c => c.Authenticate(null))
                .Throws<Exception>();

            var sut = new AuthController(authService.Object);

            var result = sut.GetToken(null);
            Assert.True(result is BadRequestObjectResult);
        }
    } 
}
