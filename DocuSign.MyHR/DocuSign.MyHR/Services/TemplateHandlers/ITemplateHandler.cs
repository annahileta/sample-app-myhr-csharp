using DocuSign.eSign.Model;
using DocuSign.MyHR.Domain;

namespace DocuSign.MyHR.Services.TemplateHandlers
{
    public interface ITemplateHandler
    {
        EnvelopeTemplate CreateTemplate(string rootDir);
        EnvelopeDefinition CreateEnvelope(UserDetails currentUser, UserDetails additionalUser);
    }
}