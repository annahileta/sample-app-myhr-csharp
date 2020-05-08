using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace DocuSign.MyHR.DocuSign.eSignature
{
    public class LocalsFilter : IActionFilter
    {
        DsConfiguration Config { get; }

        private readonly IRequestItemsService _requestItemsService;
        private IMemoryCache _cache;

        public LocalsFilter(DsConfiguration config, IRequestItemsService requestItemsService, IMemoryCache cache)
        {
            Config = config;
            _cache = cache;
            _requestItemsService = requestItemsService;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {

            Controller controller = context.Controller as Controller;

            if (controller == null)
            {
                return;
            }
            var viewBag = controller.ViewBag;
            var httpContext = context.HttpContext;

            var locals = httpContext.Session.GetObjectFromJson<Locals>("locals") ?? new Locals();
            locals.DsConfig = Config;
            locals.Session = httpContext.Session.GetObjectFromJson<Session>("session") ?? null;
            locals.Messages = "";
            locals.Json = null;
            locals.User = null;
            viewBag.Locals = locals;
            viewBag.showDoc = Config.documentation != null;


            var identity = httpContext.User.Identity as ClaimsIdentity;
            if (identity != null && !identity.IsAuthenticated)
            {
                locals.Session = new Session();
                return;
            }


            locals.User = httpContext.Session.GetObjectFromJson<User>("user");

            if (locals.User == null)
            {
                locals.User = _requestItemsService.User;
            }
            if (locals.Session == null)
            {
                locals.Session = _requestItemsService.Session;
            }
        }
    }
}