using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace DocuSign.MyHR
{
    public class ContextFilter : IActionFilter
    {
        private IMemoryCache _cache;
        private readonly Context _context;

        public ContextFilter(IMemoryCache cache, Context context)
        {
            _cache = cache;
            _context = context;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;
            if (httpContext.User.Identity.IsAuthenticated)
            {
                _context.Init(httpContext.User); 
            }
        }
    }
}