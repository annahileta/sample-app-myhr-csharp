using DocuSign.MyHR.Domain;

namespace DocuSign.MyHR.Repositories
{
    public interface ITokenRepository
    {
        void SaveToken(DocuSignToken docuSignTooken);
        DocuSignToken GetToken(string id);
    }
}