using System.Collections.Generic;
using System.IO;
using System.Linq;
using DocuSign.eSign.Model;
using DocuSign.MyHR.Domain;
using Newtonsoft.Json;

namespace DocuSign.MyHR.Services.TemplateHandlers
{
    public class OfferTemplateHandler : ITemplateHandler
    {
        private string _signerClientId = "1000";
        private string _templatePath = "/Templates/EmploymentOfferLetter.json";

        public EnvelopeTemplate CreateTemplate(string rootDir)
        {
            return JsonConvert.DeserializeObject<EnvelopeTemplate>(new StreamReader(rootDir + _templatePath).ReadToEnd());
        }

        public EnvelopeDefinition CreateEnvelope(UserDetails currentUser, UserDetails additionalUser)
        {
            EnvelopeDefinition env = new EnvelopeDefinition();

            TemplateRole roleHr = new TemplateRole
            {
                Email = currentUser.Email,
                Name = currentUser.Name,
                ClientUserId = _signerClientId,
                RoleName = "HR Rep"
            };
            TemplateRole roleNewHire = new TemplateRole
            {
                Email = additionalUser.Email,
                Name = additionalUser.Name, 
                RoleName = "New Hire"
            }; 

            env.TemplateRoles = new List<TemplateRole> { roleHr, roleNewHire };
            env.Status = "sent";
            return env;
        }
    }
}
