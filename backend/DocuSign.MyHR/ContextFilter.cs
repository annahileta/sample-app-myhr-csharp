using DocuSign.MyHR.Security;
using DocuSign.MyHR.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace DocuSign.MyHR
{
    public class ContextFilter : IActionFilter
    {
        private readonly IAuthenticationService _authenticationService;
        private IMemoryCache _cache;
        private readonly Context _context;

        public ContextFilter(IAuthenticationService authenticationService, IMemoryCache cache, Context context)
        {
            _cache = cache;
            _context = context;
            _authenticationService = authenticationService;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;

            _context.SetUser(httpContext.User);
            _context.Init(httpContext.User);
        }
    }
}