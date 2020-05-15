using System.IO;
using System.Linq;
using DocuSign.eSign.Model;
using DocuSign.MyHR.Domain;
using Newtonsoft.Json;

namespace DocuSign.MyHR.Services.TemplateHandlers
{
    public class OfferTemplateHandler
    {
        private string _signerClientId = "1000";
        private string _templatePath = "../Templates/I-EmploymentOfferLetter.json";

        public EnvelopeDefinition CreateEnvelop(UserDetails userDetails, UserDetails additionalUser, string rootDir)
        {
            var envelope = JsonConvert.DeserializeObject<EnvelopeDefinition>(new StreamReader(rootDir + _templatePath).ReadToEnd());
            
            var signerHR = envelope.Recipients.Signers.First(x => x.RoleName == "HR Rep");
            signerHR.Email = userDetails.Email;
            signerHR.Name = userDetails.Name;
            signerHR.ClientUserId = _signerClientId;

            var signerNewHire = envelope.Recipients.Signers.First(x => x.RoleName == "New Hire");
            signerNewHire.Email = additionalUser.Email;
            signerNewHire.Name = additionalUser.Name;

            envelope.Status = "Sent";

            return envelope;
        }
    }
}
