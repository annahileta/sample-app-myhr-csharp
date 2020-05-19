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

        public string CreateEnvelope(
            DocumentType type,
            string accountId,
            string userId,
            UserDetails additionalUser,
            string redirectUrl,
            string pingAction)
        {
            var rootDir = _configuration.GetValue<string>(WebHostDefaults.ContentRootKey);

            var userDetails = _userService.GetUserDetails(accountId, userId);

            EnvelopeTemplate envelopeTemplate;
            EnvelopeDefinition envelope;
            switch (type)
            {
                case DocumentType.I9: 
                    var handlerI9 = new I9TemplateHandler();
                    envelopeTemplate = handlerI9.CreateTemplate(rootDir);
                    envelope = handlerI9.CreateEnvelope(userDetails, additionalUser);
                    break;
                case DocumentType.W4:
                    var handlerW4 = new W4TemplateHandler();
                    envelopeTemplate = handlerW4.CreateTemplate(rootDir);
                    envelope = handlerW4.CreateEnvelope(userDetails);
                    break;
                case DocumentType.Offer:
                    var handlerOffer = new OfferTemplateHandler();
                    envelopeTemplate = handlerOffer.CreateTemplate(rootDir);
                    envelope = handlerOffer.CreateEnvelope(userDetails, additionalUser);
                    break;
                default:
                    throw new NotImplementedException(); 
            }

            var templateSummary = _docuSignApiProvider.TemplatesApi.CreateTemplate(accountId, envelopeTemplate); 
            envelope.TemplateId = templateSummary.TemplateId;
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
