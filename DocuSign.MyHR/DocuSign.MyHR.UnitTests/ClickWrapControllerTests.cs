using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using Moq;
using Xunit;
using AutoFixture.Xunit2;
using DocuSign.MyHR.Controllers;
using DocuSign.MyHR.Domain;
using DocuSign.MyHR.Models;
using DocuSign.MyHR.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DocuSign.MyHR.UnitTests
{
    public class ClickWrapControllerTests
    {
        [Theory, AutoData]
        public void Index_WhenGetWithWorkLogs_ReturnsCorrectResult(
            Mock<IClickWrapService> clickWrapService,
            Account account,
            User user)
        {
            // Arrange
            InitContext(account, user);
            clickWrapService.Setup(c => c.CreateTimeTrackClickWrap(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int[]>()))
                .Returns(() => new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new { clickWrapId = "1" }))
                });

            var sut = new ClickWrapController(clickWrapService.Object);

            // Act
            var result = sut.Index(new RequestClickWrapModel { WorkLogs = new[] { 1, 2, 4 } });
            
            // Assert
            Assert.True(result is OkObjectResult);
            var response = (ResponseClickWrapModel)((OkObjectResult)result).Value;
            Assert.Equal("1", (string) response.ClickWrap.clickWrapId);
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
