using DocuSign.MyHR.Domain;
using DocuSign.MyHR.Services;
using Microsoft.AspNetCore.Mvc;

namespace DocuSign.MyHR.Controllers
{
    public class EnvelopeController : Controller
    {
        private readonly IEnvelopeService _envelopeService;

        public EnvelopeController(IEnvelopeService envelopeService)
        {
            _envelopeService = envelopeService;
        }
        public IActionResult Index()
        {
            return Redirect(_envelopeService.CreateEnvelope(DocumentType.I9, Context.Account.Id, Context.User.Id));
        }
    }
}