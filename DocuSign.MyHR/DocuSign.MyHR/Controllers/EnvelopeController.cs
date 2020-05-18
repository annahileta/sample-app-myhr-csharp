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
        public IActionResult Index([FromBody] Envelope envelope)
        {
            string scheme = Url.ActionContext.HttpContext.Request.Scheme;
            return Redirect(_envelopeService.CreateEnvelope(
                envelope.Type,
                Context.Account.Id,
                Context.User.Id,
                envelope.AdditionalUser,
                envelope.RedirectUrl,
                 Url.Action("ping", "info", null, scheme)));
        }
    }
}