using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit; 
using DocumentFormat.OpenXml.Packaging;
using DocuSign.MyHR.Services;
using Microsoft.Extensions.Configuration;
using Moq.Protected;
using Newtonsoft.Json;

namespace DocuSign.MyHR.UnitTests
{
    public class ClickWrapServiceTests
    {
        private static string _accountId = "1";
        private static string _userId = "2";

        [Fact]
        public void CreateTimeTrackClickWrap_WithCorrectInput_ReturnsCorrectResponse()
        {
            //Arrange
            var docuSignApiProvider = new Mock<IDocuSignApiProvider>();
            dynamic createRequestObj = null;
            IConfiguration configuration = Setup(docuSignApiProvider, HttpStatusCode.Created, (request) =>
            {
                createRequestObj = request;
            });

            //Act
            var sut = new ClickWrapService(docuSignApiProvider.Object, configuration);
            var response = sut.CreateTimeTrackClickWrap(_accountId, _userId, new[] { 1, 2, 4, 6, 6 });

            //Assert
            var responseContent = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("1", (string)responseContent.clickwrapId);
        }

        [Fact]
        public void CreateTimeTrackClickWrap_WithCorrectInput_CallsClickWrapApiWithCorrectDocument()
        {
            //Arrange
            var docuSignApiProvider = new Mock<IDocuSignApiProvider>();
            dynamic createRequestObj = null;
            var configuration = Setup(docuSignApiProvider, HttpStatusCode.Created, (request) =>
            {
                createRequestObj = request;
            });

            //Act
            var sut = new ClickWrapService(docuSignApiProvider.Object, configuration);
            sut.CreateTimeTrackClickWrap(_accountId, _userId, new[] { 1, 2, 4, 6, 6 });

            //Assert - verify document content
            byte[] data = Convert.FromBase64String((string)createRequestObj.documents[0].documentBase64);
            using (Stream ms = new MemoryStream(data))
            {
                WordprocessingDocument wordDoc = WordprocessingDocument.Open(ms, false);
                Assert.Equal("I affirm I worked 19 hours this week.", wordDoc.MainDocumentPart.Document.Body.InnerText);
            }
        }

        [Fact]
        public void CreateTimeTrackClickWrap_WhenClickWrapIsNotCreatedByApi_ThrowsInvalidOperationException()
        {
            //Arrange
            var docuSignApiProvider = new Mock<IDocuSignApiProvider>();
            dynamic createRequestObj = null;
            var configuration = Setup(docuSignApiProvider, HttpStatusCode.BadRequest, (request) =>
            {
                createRequestObj = request;
            });

            //Act
            Assert.Throws<InvalidOperationException>(() => new ClickWrapService(docuSignApiProvider.Object, configuration)
                .CreateTimeTrackClickWrap(_accountId, _userId, new[] { 5, 5, 6, 8, 7 }));
        }

        [Fact]
        public void CreateTimeTrackClickWrap_WhenAccountIdIsNull_ThrowsArgumentNullException()
        {
            //Arrange
            var docuSignApiProvider = new Mock<IDocuSignApiProvider>();
            dynamic createRequestObj = null;
            var configuration = Setup(docuSignApiProvider, HttpStatusCode.Created, (request) =>
            {
                createRequestObj = request;
            });

            //Act
            Assert.Throws<ArgumentNullException>(() => new ClickWrapService(docuSignApiProvider.Object, configuration)
                .CreateTimeTrackClickWrap(null, _userId, new[] { 5, 5, 6, 8, 7 }));
        }

        [Fact]
        public void CreateTimeTrackClickWrap_WhenUserIdIsNull_ThrowsArgumentNullException()
        {
            //Arrange
            var docuSignApiProvider = new Mock<IDocuSignApiProvider>();
            dynamic createRequestObj = null;
            var configuration = Setup(docuSignApiProvider, HttpStatusCode.Created, (request) =>
            {
                createRequestObj = request;
            });

            //Act
            Assert.Throws<ArgumentNullException>(() => new ClickWrapService(docuSignApiProvider.Object, configuration)
                .CreateTimeTrackClickWrap(_accountId, null, new[] { 5, 5, 6, 8, 7 }));
        }

        [Fact]
        public void CreateTimeTrackClickWrap_WhenWorkLogIsNull_ThrowsArgumentNullException()
        {
            //Arrange
            var docuSignApiProvider = new Mock<IDocuSignApiProvider>();
            dynamic createRequestObj = null;
            var configuration = Setup(docuSignApiProvider, HttpStatusCode.Created, (request) =>
            {
                createRequestObj = request;
            });

            //Act
            Assert.Throws<ArgumentNullException>(() => new ClickWrapService(docuSignApiProvider.Object, configuration)
                .CreateTimeTrackClickWrap(_accountId, _userId, null));
        }

        [Fact]
        public void CreateTimeTrackClickWrap_WhenWorkLogDoesNotContainAllWorkingDays_ThrowsInvalidOperationException()
        {
            //Arrange
            var docuSignApiProvider = new Mock<IDocuSignApiProvider>();
            dynamic createRequestObj = null;
            var configuration = Setup(docuSignApiProvider, HttpStatusCode.Created, (request) =>
            {
                createRequestObj = request;
            });

            //Act
            Assert.Throws<InvalidOperationException>(() => new ClickWrapService(docuSignApiProvider.Object, configuration)
                .CreateTimeTrackClickWrap(_accountId, _userId, new[] { 5, 5, 6 }));
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
