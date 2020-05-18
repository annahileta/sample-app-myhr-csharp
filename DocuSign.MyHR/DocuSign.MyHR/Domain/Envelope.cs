using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocuSign.MyHR.Domain
{
    public class Envelope
    {
        public Envelope()
        {
                
        }
        public DocumentType Type { get; set; }
        public UserDetails AdditionalUser { get; set; }
        public string RedirectUrl { get; set; }
    }
}
