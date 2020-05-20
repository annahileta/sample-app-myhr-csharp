using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using AutoFixture.Xunit2;
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
            dynamic createRequestObj = null;
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync((HttpRequestMessage a, CancellationToken b) =>
                {
                    if (a.Method == HttpMethod.Post)
                    {
                        createRequestObj = JsonConvert.DeserializeObject<dynamic>(a.Content.ReadAsStringAsync().Result);
                    }
                    return new HttpResponseMessage
                    {
                        StatusCode = a.Method == HttpMethod.Post ? HttpStatusCode.Created : HttpStatusCode.OK,
                        Content = new StringContent(JsonConvert.SerializeObject(new { clickwrapId = "1" }))
                    };
                });

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"contentRoot",   "../../../../DocuSign.MyHR/"},
                })
                .Build();

            var httpClient = new HttpClient(mockMessageHandler.Object) { BaseAddress = new Uri("http://localhost") };
            docuSignApiProvider.SetupGet(c => c.DocuSignHttpClient).Returns(httpClient);

            var sut = new ClickWrapService(docuSignApiProvider.Object, configuration);

            var response = sut.CreateTimeTrackClickWrap(accountId, userId, new[] { 1, 2, 4 });
            var responseContent = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("Time Tracking Confirmation", (string)createRequestObj.documents[0].documentName);
            Assert.NotNull(createRequestObj.documents[0].documentBase64);
            Assert.Equal("1", (string)responseContent.clickwrapId);
        }
    }
}
