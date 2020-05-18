using DocuSign.MyHR.Domain;

namespace DocuSign.MyHR.Models
{
    public class RequestClickWrapModel
    {
        public int[] WorkLogs { get; set; }
        public UserDetails AdditionalUser { get; set; }
        public string RedirectUrl { get; set; }
    }
}