using System.Collections.Generic;
using System.IO;
using DocuSign.eSign.Model;
using DocuSign.MyHR.Domain;
using Newtonsoft.Json;

namespace DocuSign.MyHR.Services.TemplateHandlers
{
    public class I9TemplateHandler : ITemplateHandler
    {
        private string _templatePath = "/Templates/I-9_2020.json";

        public EnvelopeTemplate BuildTemplate(string rootDir)
        {
            return JsonConvert.DeserializeObject<EnvelopeTemplate>(new StreamReader(rootDir + _templatePath).ReadToEnd());
        }

        public EnvelopeDefinition BuildEnvelope(UserDetails currentUser, UserDetails additionalUser)
        {
            EnvelopeDefinition env = new EnvelopeDefinition();

            TemplateRole roleHr = new TemplateRole
            {
                Email = currentUser.Email,
                Name = currentUser.Name,
                RoleName = "HR"
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
