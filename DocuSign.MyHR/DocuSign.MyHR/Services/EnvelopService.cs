using System;
using System.IO;
using System.Linq;
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
        private string _w4TemplatePath = "/Templates/W-4_2020.json";

        private readonly IDocuSignApiProvider _docuSignApiProvider;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public EnvelopeService(IDocuSignApiProvider docuSignApiProvider, IUserService userService, IConfiguration configuration)
        {
            _docuSignApiProvider = docuSignApiProvider;
            _userService = userService;
            _configuration = configuration;
        }

        public string CreateEnvelope(
            DocumentType type,
            string accountId,
            string userId, 
            string redirectUrl,
            string pingAction)
        {
            var userDetails = _userService.GetUserDetails(accountId, userId);
            var rootDir = _configuration.GetValue<string>(WebHostDefaults.ContentRootKey);
           
            EnvelopeDefinition envelope;
            switch (type)
            {
                case DocumentType.I9:
                    envelope = JsonConvert.DeserializeObject<EnvelopeDefinition>(new StreamReader(rootDir +_i9TemplatePath).ReadToEnd());
                    break;
                case DocumentType.W4:
                    envelope = JsonConvert.DeserializeObject<EnvelopeDefinition>(new StreamReader(rootDir + _w4TemplatePath).ReadToEnd());
                    break;
                default:
                    throw new NotImplementedException(); 
            }

            var signerNewHire = envelope.Recipients.Signers.First(x => x.RoleName == "New Hire");
            signerNewHire.Email = userDetails.Email;
            signerNewHire.Name = userDetails.Name;
            signerNewHire.ClientUserId = _signerClientId;

            var signerHR = envelope.Recipients.Signers.First(x => x.RoleName == "HR");
            signerHR.Email = userDetails.Email;
            signerHR.Name = userDetails.Name;
            signerHR.ClientUserId = _signerClientId;
            envelope.Status = "Sent";  

            EnvelopeSummary results = _docuSignApiProvider.EnvelopApi.CreateEnvelope(accountId, envelope);
            string envelopeId = results.EnvelopeId;

            RecipientViewRequest viewRequest = MakeRecipientViewRequest(
                userDetails.Email, 
                userDetails.Name,
                redirectUrl, 
                _signerClientId,
                pingAction);
             
            ViewUrl results1 = _docuSignApiProvider.EnvelopApi.CreateRecipientView(accountId, envelopeId, viewRequest);

            return results1.Url;
        } 

        private static RecipientViewRequest MakeRecipientViewRequest(string signerEmail, string signerName, string returnUrl, string signerClientId, string pingUrl = null)
        {
            RecipientViewRequest viewRequest = new RecipientViewRequest
            {
                ReturnUrl = returnUrl,
                AuthenticationMethod = "none",
                Email = signerEmail,
                UserName = signerName,
                ClientUserId = signerClientId
            };

            if (pingUrl != null)
            {
                viewRequest.PingFrequency = "600";
                viewRequest.PingUrl = pingUrl;
            }

            return viewRequest;
        }
    }
}
