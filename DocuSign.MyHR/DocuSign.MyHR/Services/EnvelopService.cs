using System;
using DocuSign.eSign.Model;
using DocuSign.MyHR.Domain;
using DocuSign.MyHR.Services.TemplateHandlers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace DocuSign.MyHR.Services
{
    public class EnvelopeService : IEnvelopeService
    {
        private string _signerClientId = "1000"; 
        private readonly IDocuSignApiProvider _docuSignApiProvider;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public EnvelopeService(IDocuSignApiProvider docuSignApiProvider, IUserService userService, IConfiguration configuration)
        {
            _docuSignApiProvider = docuSignApiProvider;
            _userService = userService;
            _configuration = configuration;
        }

        public string CreateEnvelope(DocumentType type,
            string accountId,
            string userId,
            UserDetails additionalUser,
            string redirectUrl,
            string pingAction)
        {
            var rootDir = _configuration.GetValue<string>(WebHostDefaults.ContentRootKey);

            var userDetails = _userService.GetUserDetails(accountId, userId);
           
            EnvelopeDefinition envelope;
            switch (type)
            {
                case DocumentType.I9:
                    envelope = new I9TemplateHandler().CreateEnvelop(userDetails, additionalUser, rootDir);
                    break;
                case DocumentType.W4:
                    envelope = new W4TemplateHandler().CreateEnvelop(userDetails, rootDir);
                    break;
                case DocumentType.Offer:
                    envelope = new OfferTemplateHandler().CreateEnvelop(userDetails, additionalUser, rootDir);
                    break;
                default:
                    throw new NotImplementedException(); 
            }
             
            EnvelopeSummary results = _docuSignApiProvider.EnvelopApi.CreateEnvelope(accountId, envelope);
            string envelopeId = results.EnvelopeId;

            RecipientViewRequest viewRequest = MakeRecipientViewRequest(
                userDetails.Email, 
                userDetails.Name,
                redirectUrl, 
                _signerClientId,
                pingAction);
             
            ViewUrl recipientView = _docuSignApiProvider.EnvelopApi.CreateRecipientView(accountId, envelopeId, viewRequest);

            return recipientView.Url;
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
