using DocuSign.MyHR.Domain;
using DocuSign.MyHR.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocuSign.MyHR.Controllers
{
    [Authorize]
    public class EnvelopeController : Controller
    {
        private readonly IEnvelopeService _envelopeService;

        public EnvelopeController(IEnvelopeService envelopeService)
        {
            _envelopeService = envelopeService;
        }

        [HttpPost]
        public IActionResult Index(DocumentType type, UserDetails additionalUser, string redirectUrl)
        {
            string scheme = Url.ActionContext.HttpContext.Request.Scheme;
            return Redirect(_envelopeService.CreateEnvelope(
                type,
                Context.Account.Id,
                Context.User.Id,
                additionalUser,
                redirectUrl,
                 Url.Action("ping", "info", null, scheme)));
        }
    }
}