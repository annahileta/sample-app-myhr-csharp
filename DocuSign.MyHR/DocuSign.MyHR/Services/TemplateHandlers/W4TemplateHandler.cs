using System;
using System.Collections.Generic;
using System.IO; 
using DocuSign.eSign.Model;
using DocuSign.MyHR.Domain;
using Newtonsoft.Json;

namespace DocuSign.MyHR.Services.TemplateHandlers
{
    public class W4TemplateHandler : ITemplateHandler
    {
        private string _signerClientId = "1000";
        private string _templatePath = "/Templates/W-4_2020.json";

        public EnvelopeTemplate BuildTemplate(string rootDir)
        {
            return JsonConvert.DeserializeObject<EnvelopeTemplate>(new StreamReader(rootDir + _templatePath).ReadToEnd());
        }
         
        public EnvelopeDefinition BuildEnvelope(UserDetails currentUser, UserDetails additionalUser)
        {
            EnvelopeDefinition env = new EnvelopeDefinition();

            TemplateRole role = new TemplateRole
            {
                Email = currentUser.Email,
                Name = currentUser.Name,
                RoleName = "New Hire",
                ClientUserId = _signerClientId
            };

            env.TemplateRoles = new List<TemplateRole> { role };
            env.Status = "sent";
            return env;
        } 
    }
}
