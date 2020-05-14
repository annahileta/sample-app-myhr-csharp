using System.IO;
using System.Linq;
using DocuSign.eSign.Model;
using DocuSign.MyHR.Domain;
using Newtonsoft.Json;

namespace DocuSign.MyHR.Services
{
    public class W4TemplateHandler
    {
        private string _signerClientId = "1000";
        private string _templatePath = "/Templates/W-4_2020.json";

        public EnvelopeDefinition CreateEnvelop(UserDetails userDetails)
        {
            var envelope = JsonConvert.DeserializeObject<EnvelopeDefinition>(new StreamReader(_templatePath).ReadToEnd());
              
            var signerNewHire = envelope.Recipients.Signers.First(x => x.RoleName == "New Hire");
            signerNewHire.Email = userDetails.Email;
            signerNewHire.Name = userDetails.Name;
            signerNewHire.ClientUserId = _signerClientId;

            var signerHR = envelope.Recipients.Signers.First(x => x.RoleName == "HR");
            signerHR.Email = userDetails.Email;
            signerHR.Name = userDetails.Name;
            signerHR.ClientUserId = _signerClientId;
            envelope.Status = "Sent";

            return envelope;
        }
    }
}
