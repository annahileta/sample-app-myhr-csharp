using System.IO;
using DocuSign.eSign.Model;
using Newtonsoft.Json;

namespace DocuSign.MyHR.Services
{
    public class I9TemplateHandler
    {
        private string _templatePath = "../Templates/I-9_2020.json";

        public void CreateEnvelop()
        {
            var definition = JsonConvert.DeserializeObject<EnvelopeDefinition>(new StreamReader(_templatePath).ReadToEnd());

        }
    }
}
