using System;
using DocuSign.MyHR.Repositories;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using AutoFixture;
using AutoFixture.Xunit2;
using DocuSign.MyHR.Services;

namespace DocuSign.MyHR.UnitTests
{
    public class AuthenticationServiceTests
    {
        [Theory, AutoData]
        public void GetAuthorizationUrl_ReturnsCorrectUrl(
            Mock<ITokenRepository> tokenRepository, 
            Mock<IConfiguration> configurationService)
        {
            configurationService.SetupGet(c => c["DocuSign:IntegrationKey"]).Returns("1111-111-integrationKey");
            configurationService.SetupGet(c => c["DocuSign:AuthServer"]).Returns("test.docusign.authserver");
            var sut = new AuthenticationService(tokenRepository.Object, configurationService.Object);

            string url = sut.GetAuthorizationUrl("http://test.test");
            Assert.Equal(
                "https://test.docusign.authserver/oauth/auth?&scope=signature&client_id=1111-111-integrationKey&redirect_uri=http://test.test", 
                url);
        }
    }
}
