using System;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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
                $"clickapi/v1/accounts/{accountId}/clickwraps"); 

            var requestBody = new
            {
                displaySettings = new
                {
                    consentButtonText = "I Confirm",
                    hasDeclineButton = true,
                    displayName = "Time Tracking Confirmation",
                    downloadable = true,
                    format = "modal",
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
                        documentBase64 = GetDocumentBase64(workingLog, rootDir),
                        documentName = "Time Tracking Confirmation",
                        fileExtension = "docx",
                        order = 0
                    }
                },
                name = "Time Tracking Confirmation",
                status = "active"
            };
            request.Content = new StringContent(
                JsonConvert.SerializeObject(requestBody),
                Encoding.UTF8,
                "application/json"); 

            return _docuSignApiProvider.DocuSignHttpClient.SendAsync(request).Result;
        }

        private string GetDocumentBase64(int[] workingLog, string rootDir)
        {
            string docBase64;
            byte[] byteArray = File.ReadAllBytes(rootDir + _templatePath);
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(byteArray, 0, (int) byteArray.Length);
                using (WordprocessingDocument doc = WordprocessingDocument.Open(stream, true))
                {
                    var body = doc.MainDocumentPart.Document.Body;

                    foreach (var text in body.Descendants<Text>())
                    {
                        if (text.Text.Contains("hrs"))
                        {
                            text.Text = text.Text.Replace("hrs", workingLog.Sum() + " hrs");
                        }
                    }
                    doc.Save(); 
                    using (StreamReader sr = new StreamReader(doc.MainDocumentPart.GetStream()))
                    {
                        docBase64 = Convert.ToBase64String(stream.ToArray());
                    }

                   
                }
            }

            return docBase64;
        }
    }
}
