using System.Collections.Generic;
using Moq;
using Xunit;
using AutoFixture.Xunit2;
using DocuSign.eSign.Api;
using DocuSign.eSign.Model;
using DocuSign.MyHR.Domain;
using DocuSign.MyHR.Services;
using Microsoft.Extensions.Configuration;

namespace DocuSign.MyHR.UnitTests
{
    public class EnvelopeServiceTests
    {
        [Theory]
        [InlineAutoData(DocumentType.W4)]
        [InlineAutoData(DocumentType.I9)]
        [InlineAutoData(DocumentType.Offer)]
        public void CreateEnvelope_ReturnsCorrectResult_W4(
            DocumentType type,
            Mock<IDocuSignApiProvider> docuSignApiProvider,
            Mock<IUserService> userService,
            UserDetails userInformation,
            UserDetails additionalUser,
            string accountId,
            string userId)
        {
            SutupApi(docuSignApiProvider, userInformation, accountId, userId);
            userService.Setup(x => x.GetUserDetails(accountId, userId)).Returns(userInformation);

            var inMemorySettings = new Dictionary<string, string>
            {
                {"contentRoot",   "../../../../DocuSign.MyHR/"},
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
            var sut = new EnvelopeService(docuSignApiProvider.Object, userService.Object, configuration);

            string envelopeUrl = sut.CreateEnvelope(type, accountId, userId, additionalUser, "", "");
            Assert.NotNull(envelopeUrl);
            Assert.Equal($"accountId={accountId}&templateId=1&userEmail={userInformation.Email}&userName={userInformation.Name}", envelopeUrl);
        } 

        private static void SutupApi(Mock<IDocuSignApiProvider> docuSignApiProvider, UserDetails userInformation, string accountId, string userId)
        { 
            var envelopeApi = new Mock<IEnvelopesApi>();
            envelopeApi.Setup(x =>
                    x.CreateEnvelope(accountId, It.IsAny<EnvelopeDefinition>(), It.IsAny<EnvelopesApi.CreateEnvelopeOptions>()))
                .Returns((string a, EnvelopeDefinition c, EnvelopesApi.CreateEnvelopeOptions o) =>
                    new EnvelopeSummary(EnvelopeId: "1"));
            envelopeApi.Setup(x => x.CreateRecipientView(accountId, "1", It.IsAny<RecipientViewRequest>()))
                .Returns((string a, string b, RecipientViewRequest c) =>
                    new ViewUrl($"accountId={a}&templateId={b}&userEmail={c.Email}&userName={c.UserName}"));

            docuSignApiProvider.SetupGet(c => c.EnvelopApi).Returns(envelopeApi.Object);
             
            var templateApi = new Mock<ITemplatesApi>();
            templateApi.Setup(x => x.CreateTemplate(accountId, It.IsAny<EnvelopeTemplate>()))
                .Returns((string a, EnvelopeTemplate b) => new TemplateSummary(DocumentName: b.Name, TemplateId: "1"));

            docuSignApiProvider.SetupGet(c => c.TemplatesApi).Returns(templateApi.Object);
        }
    }
}
