using System.Collections.Generic;
using System.Security.Claims;
using Moq;
using Xunit;
using AutoFixture.Xunit2;
using DocuSign.MyHR.Controllers;
using DocuSign.MyHR.Domain;
using DocuSign.MyHR.Models;
using DocuSign.MyHR.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;

namespace DocuSign.MyHR.UnitTests
{
    public class EnvelopeControllerTests
    {
        [Theory, AutoData]
        public void Index_Get_ReturnsCorrectResult(
            Mock<IEnvelopeService> envelopeService,
            Account account,
            User user,
            UserDetails additionalUser)
        {
            //Arrange
            InitContext(account, user);
            envelopeService.Setup(c => c.CreateEnvelope(
                    It.IsAny<DocumentType>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<LoginType>(),
                    It.IsAny<UserDetails>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(() => new CreateEnvelopeResponse("envelopeUrl", "1"));

            var sut = new EnvelopeController(envelopeService.Object);
            var urlHelperMock = new Mock<IUrlHelper>();
            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(x => x.Request).Returns(() => new Mock<HttpRequest>().Object);

            var actionContext = new ActionContext(
                httpContext.Object,
                Mock.Of<RouteData>(),
                Mock.Of<ActionDescriptor>(),
                new ModelStateDictionary()
            );
            urlHelperMock.SetupGet(x => x.ActionContext).Returns(actionContext).Verifiable();
            sut.Url = urlHelperMock.Object;

            //Act
            var result = sut.Index(new RequestEnvelopeModel { AdditionalUser = additionalUser, RedirectUrl = "/", Type = DocumentType.I9 });

            //Assert
            Assert.True(result is OkObjectResult);
            var response = (ResponseEnvelopeModel)((OkObjectResult)result).Value;
            Assert.Equal("envelopeUrl", response.RedirectUrl);
            Assert.Equal("1", response.EnvelopeId);
        }

        private void InitContext(Account account, User user)
        {
            var context = new Context();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Name),
            };
            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            claimsIdentity.AddClaim(new Claim("accounts", JsonConvert.SerializeObject(account)));
            claimsIdentity.AddClaim(new Claim("account_id", account.Id));
            claimsIdentity.AddClaim(new Claim("authType", LoginType.CodeGrant.ToString()));

            context.Init(new ClaimsPrincipal(claimsIdentity));
        }
    }
}
