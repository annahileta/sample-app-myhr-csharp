using DocuSign.MyHR.Domain;
using DocuSign.MyHR.Security;

namespace DocuSign.MyHR.Repositories
{
    public interface ITokenRepository
    {
        void SaveToken(DocuSignToken docuSignTooken);
        DocuSignToken GetDocuSignToken(string userId);
        void RemoveTokens(string userId);
    }
}