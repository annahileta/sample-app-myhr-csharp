using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DocuSign.eSign.Model;
using DocuSign.MyHR.Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace DocuSign.MyHR.Services
{
    public class EnvelopeService : IEnvelopeService
    {
        private string _signerClientId = "1000";
        private string _i9TemplatePath = "/Templates/I-9_2020.json";

        private readonly IDocuSignApiProvider _docuSignApiProvider;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public EnvelopeService(IDocuSignApiProvider docuSignApiProvider, IUserService userService, IConfiguration configuration)
        {
            _docuSignApiProvider = docuSignApiProvider;
            _userService = userService;
            _configuration = configuration;
        }

        public string CreateEnvelope(DocumentType type, string accountId, string userId)
        {
            var userDetails = _userService.GetUserDetails(accountId, userId);
            var rootDir = _configuration.GetValue<string>(WebHostDefaults.ContentRootKey);
           
            EnvelopeDefinition envelope;
            switch (type)
            {
                case DocumentType.I9:
                    envelope = JsonConvert.DeserializeObject<EnvelopeDefinition>(new StreamReader(rootDir +_i9TemplatePath).ReadToEnd());
                    break;
                default:
                    throw new NotImplementedException(); 
            }

            envelope.TemplateRoles = new List<TemplateRole> 
            {
                new TemplateRole
                {
                    Email = userDetails.Email,
                    Name = userDetails.Name,
                    RoleName = "New Hire",
                }
            };

            EnvelopeSummary results = _docuSignApiProvider.EnvelopApi.CreateEnvelope(accountId, envelope);
            string envelopeId = results.EnvelopeId;

            RecipientViewRequest viewRequest = MakeRecipientViewRequest(
                userDetails.Email, 
                userDetails.Name, 
                "https://localhost:5001", 
                _signerClientId,
                "https://localhost:5001/info/ping");
             
            ViewUrl results1 = _docuSignApiProvider.EnvelopApi.CreateRecipientView(accountId, envelopeId, viewRequest);

            return results1.Url;
        } 

        private static RecipientViewRequest MakeRecipientViewRequest(string signerEmail, string signerName, string returnUrl, string signerClientId, string pingUrl = null)
        {

            RecipientViewRequest viewRequest = new RecipientViewRequest();

            viewRequest.ReturnUrl = returnUrl + "?state=123";

            viewRequest.AuthenticationMethod = "none";
             
            viewRequest.Email = signerEmail;
            viewRequest.UserName = signerName;
            viewRequest.ClientUserId = signerClientId;

            if (pingUrl != null)
            {
                viewRequest.PingFrequency = "600";
                viewRequest.PingUrl = pingUrl;
            }

            return viewRequest;
        }
    }


}
