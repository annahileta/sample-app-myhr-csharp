using System.Collections.Generic;
using DocuSign.MyHR.Domain;

namespace DocuSign.MyHR.Services
{
    public interface IEnvelopeService
    {
        string CreateEnvelope(
            DocumentType type, 
            string accountId,
            string userId, 
            UserDetails additionalUser,
            string redirectUrl,
            string pingAction);

        Dictionary<string, string> GetEnvelopData(
            string accountId,
            string envelopId);
    }
}