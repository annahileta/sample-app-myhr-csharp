using System.Collections.Generic;
using System.Linq;
using Moq;
using Xunit;
using AutoFixture.Xunit2;
using DocuSign.eSign.Api;
using DocuSign.eSign.Model;
using DocuSign.MyHR.Domain;
using DocuSign.MyHR.Services;
using DocuSign.MyHR.Services.TemplateHandlers;
using Microsoft.Extensions.Configuration;

namespace DocuSign.MyHR.UnitTests
{
    public class EnvelopeServiceTests
    {
        [Theory]
        [InlineAutoData(DocumentType.W4)]
        [InlineAutoData(DocumentType.Offer)]
        [InlineAutoData(DocumentType.DirectDeposit)]
        public void CreateEnvelope_ReturnsCorrectResult(
            DocumentType type,
            Mock<IDocuSignApiProvider> docuSignApiProvider,
            Mock<IUserService> userService,
            UserDetails userInformation,
            UserDetails additionalUser,
            string accountId,
            string userId)
        {
            //Arrange
            SetupApi(docuSignApiProvider, accountId);
            userService.Setup(x => x.GetUserDetails(accountId, userId)).Returns(userInformation);

            var sut = new EnvelopeService(docuSignApiProvider.Object, userService.Object, SetupConfiguration());
           
            //Act
            CreateEnvelopeResponse res = sut.CreateEnvelope(type, accountId, userId, LoginType.CodeGrant, additionalUser, "", "");
            
            //Assert
            Assert.NotNull(res);
            Assert.Equal($"accountId={accountId}&templateId=1&userEmail={userInformation.Email}&userName={userInformation.Name}", res.RedirectUrl);
            Assert.Equal("1", res.EnvelopeId);
        }

        [Theory]
        [AutoData]
        public void CreateEnvelope_I9_JWT_ReturnsCorrectResult_WithEnabledIdv(
            Mock<IDocuSignApiProvider> docuSignApiProvider,
            Mock<IUserService> userService,
            UserDetails userInformation,
            UserDetails additionalUser,
            string accountId,
            string userId)
        {
            //Arrange
            SetupApi(docuSignApiProvider, accountId);
            userService.Setup(x => x.GetUserDetails(accountId, userId)).Returns(userInformation);

            var accountsApi = new Mock<IAccountsApi>();
            accountsApi.Setup(x => x.GetAccountIdentityVerification(accountId))
                .Returns(() =>
                    new AccountIdentityVerificationResponse
                    {
                        IdentityVerification = new List<AccountIdentityVerificationWorkflow>
                        {
                            new AccountIdentityVerificationWorkflow(WorkflowId: "100")
                        }
                    });
            docuSignApiProvider.SetupGet(c => c.AccountsApi).Returns(accountsApi.Object);

            var sut = new EnvelopeService(docuSignApiProvider.Object, userService.Object, SetupConfiguration());

            //Act
            CreateEnvelopeResponse res = sut.CreateEnvelope(DocumentType.I9, accountId, userId, LoginType.JWT, additionalUser, "", "");
            
            //Assert
            Assert.NotNull(res);
            Assert.Equal(string.Empty, res.RedirectUrl);
            Assert.Equal("1", res.EnvelopeId);

            //Asset Created template - IDV must be enabled
            var templateToExpect = new I9TemplateHandler().BuildTemplate("../../../../DocuSign.MyHR/");
            templateToExpect.Recipients.Signers.First().IdentityVerification = new RecipientIdentityVerification(WorkflowId: "100");
            docuSignApiProvider.Verify(mock => mock.TemplatesApi.CreateTemplate(accountId, templateToExpect), Times.Once());
        }

        private static IConfiguration SetupConfiguration()
        {
            var inMemorySettings = new Dictionary<string, string>
            {
                {"contentRoot", "../../../../DocuSign.MyHR/"},
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
            return configuration;
        }

        private static void SetupApi(Mock<IDocuSignApiProvider> docuSignApiProvider, string accountId)
        {
            var envelopeApi = new Mock<IEnvelopesApi>();
            
            envelopeApi.Setup(x => x.CreateEnvelope(accountId, It.IsAny<EnvelopeDefinition>(), It.IsAny<EnvelopesApi.CreateEnvelopeOptions>()))
                .Returns(() => new EnvelopeSummary(EnvelopeId: "1"));

            envelopeApi.Setup(x => x.CreateRecipientView(accountId, "1", It.IsAny<RecipientViewRequest>()))
                .Returns((string a, string b, RecipientViewRequest c) =>
                    new ViewUrl($"accountId={a}&templateId={b}&userEmail={c.Email}&userName={c.UserName}"));

            docuSignApiProvider.SetupGet(c => c.EnvelopApi).Returns(envelopeApi.Object);

            var templateApi = new Mock<ITemplatesApi>();
            
            templateApi.Setup(x => x.CreateTemplate(accountId, It.IsAny<EnvelopeTemplate>()))
                .Returns((string a, EnvelopeTemplate b) =>
                    new TemplateSummary(DocumentName: b.Name, TemplateId: "1"));

            docuSignApiProvider.SetupGet(c => c.TemplatesApi).Returns(templateApi.Object);
        }
    }
}
