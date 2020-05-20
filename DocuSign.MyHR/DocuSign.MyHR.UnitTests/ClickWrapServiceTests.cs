using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using AutoFixture.Xunit2;
using DocumentFormat.OpenXml.Packaging;
using DocuSign.MyHR.Services;
using Microsoft.Extensions.Configuration;
using Moq.Protected;
using Newtonsoft.Json;

namespace DocuSign.MyHR.UnitTests
{
    public class ClickWrapServiceTests
    {
        [Theory, AutoData]
        public void CreateTimeTrackClickWrap_ReturnsCorrectResult(
            Mock<IDocuSignApiProvider> docuSignApiProvider,
            string accountId,
            string userId)
        {
            //Arrange
            dynamic createRequestObj = null;
            var configuration = Setup(docuSignApiProvider, HttpStatusCode.Created, (request)=>
            {
                createRequestObj = request;
            });

            //Act
            var sut = new ClickWrapService(docuSignApiProvider.Object, configuration);
            var response = sut.CreateTimeTrackClickWrap(accountId, userId, new[] { 1, 2, 4 });
            var responseContent = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);

            //Assert
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("Time Tracking Confirmation", (string)createRequestObj.documents[0].documentName);
            Assert.NotNull(createRequestObj.documents[0].documentBase64);
            byte[] data = Convert.FromBase64String((string)createRequestObj.documents[0].documentBase64);
            using (Stream ms = new MemoryStream(data))
            {
                WordprocessingDocument wordDoc = WordprocessingDocument.Open(ms, false);
                Assert.Equal("I affirm I worked 7 hours this week.", wordDoc.MainDocumentPart.Document.Body.InnerText);
            }
            Assert.Equal("1", (string)responseContent.clickwrapId);
        }

        [Theory, AutoData]
        public void CreateTimeTrackClickWrap_When(
         Mock<IDocuSignApiProvider> docuSignApiProvider,
         string accountId,
         string userId)
        {
            //Arrange
            dynamic createRequestObj = null;
            var configuration = Setup(docuSignApiProvider, HttpStatusCode.BadRequest, (request) =>
            {
                createRequestObj = request;
            });

            //Act
            Assert.Throws<InvalidOperationException>(() => new ClickWrapService(docuSignApiProvider.Object, configuration)
                .CreateTimeTrackClickWrap(accountId, userId, new[] { 1, 2, 4 }));
        }

        private IConfiguration Setup(Mock<IDocuSignApiProvider> docuSignApiProvider, HttpStatusCode createClickwrapStatusCode, Action<dynamic> setRequest)
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync((HttpRequestMessage a, CancellationToken b) =>
                {
                    if (a.Method == HttpMethod.Post)
                    {
                        setRequest(JsonConvert.DeserializeObject<dynamic>(a.Content.ReadAsStringAsync().Result));
                    }

                    return new HttpResponseMessage
                    {
                        StatusCode = a.Method == HttpMethod.Post ? createClickwrapStatusCode : HttpStatusCode.OK,
                        Content = new StringContent(JsonConvert.SerializeObject(new { clickwrapId = "1" }))
                    };
                });

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"contentRoot", "../../../../DocuSign.MyHR/"},
                })
                .Build();

            var httpClient = new HttpClient(mockMessageHandler.Object) { BaseAddress = new Uri("http://localhost") };
            docuSignApiProvider.SetupGet(c => c.DocuSignHttpClient).Returns(httpClient);
            return configuration;
        }
    }
}
