using System;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using System.Net.Http;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace DocuSign.MyHR.Services
{
    public class ClickWrapService : IClickWrapService
    {
        private readonly IDocuSignApiProvider _docuSignApiProvider;
        private readonly IConfiguration _configuration;
        private string _templatePath = "/Templates/Time Tracking Confirmation.docx";

        public ClickWrapService(IDocuSignApiProvider docuSignApiProvider, IConfiguration configuration)
        {
            _docuSignApiProvider = docuSignApiProvider;
            _configuration = configuration;
        }

        public HttpResponseMessage CreateTimeTrackClickWrap(string accountId, int[] workingLog)
        {
            var rootDir = _configuration.GetValue<string>(WebHostDefaults.ContentRootKey);

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"/v1/accounts/{accountId}/clickwraps");

            request.Headers.Add("Accept", "application/vnd.github.v3+json");
            request.Headers.Add("User-Agent", "MyHR");
            string docBase64;
            byte[] byteArray = File.ReadAllBytes(rootDir + _templatePath);
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(byteArray, 0, (int)byteArray.Length);
                using (WordprocessingDocument doc = WordprocessingDocument.Open(stream, true))
                {
                    var body = doc.MainDocumentPart.Document.Body;

                    foreach (var text in body.Descendants<Text>())
                    {
                        if (text.Text.Contains("{hrs}"))
                        {
                            text.Text = text.Text.Replace("{hrs}", workingLog.Sum().ToString());
                        }
                    }
                    docBase64 = Convert.ToBase64String(stream.ToArray());
                }
            }

            var requestBody = new
            {
                displaySettings = new
                {
                    consentButtonText = "string",
                    displayName = "Time Tracking Confirmation",
                    downloadable = true,
                    format = "docx",
                    hasAccep = true,
                    mustRead = true,
                    mustView = true,
                    requireAccept = true,
                    documentDisplay = "document",
                    Size = "small"
                },
                documents = new[]
                {
                    new
                    {
                        documentBase64 = docBase64,
                        documentName = "Time Tracking Confirmation",
                        fileExtension = "docx",
                        order = 0
                    }
                },
                name = "Time Tracking Confirmation",
                Status = "active"
            };
            request.Content = new StringContent(JsonConvert.SerializeObject(requestBody));
            JsonConvert.SerializeObject(requestBody);

            return _docuSignApiProvider.DocuSignHttpClient.SendAsync(request).Result;
        }
    }
}
