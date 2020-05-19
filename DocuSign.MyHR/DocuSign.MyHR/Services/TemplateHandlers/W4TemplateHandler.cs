using System;
using System.Collections.Generic;
using System.IO; 
using DocuSign.eSign.Model;
using DocuSign.MyHR.Domain;
using Newtonsoft.Json;

namespace DocuSign.MyHR.Services.TemplateHandlers
{
    public class W4TemplateHandler
    {
        private string _signerClientId = "1000";
        private string _templatePath = "/Templates/W-4_2020.json";

        public EnvelopeTemplate CreateTemplate(string rootDir)
        {
            return JsonConvert.DeserializeObject<EnvelopeTemplate>(new StreamReader(rootDir + _templatePath).ReadToEnd());
        }

        public EnvelopeDefinition CreateEnvelope(UserDetails userDetails)
        {
            EnvelopeDefinition env = new EnvelopeDefinition();

            TemplateRole role = new TemplateRole
            {
                Email = userDetails.Email,
                Name = userDetails.Name,
                RoleName = "New Hire",
                ClientUserId = _signerClientId
            };

            env.TemplateRoles = new List<TemplateRole> { role };
            env.Status = "sent";
            return env;
        }
    }
}
