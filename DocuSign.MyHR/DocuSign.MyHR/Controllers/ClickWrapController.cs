using DocuSign.MyHR.Models;
using DocuSign.MyHR.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DocuSign.MyHR.Controllers
{
    [Authorize]
    public class ClickWrapController : Controller
    {
        private readonly IClickWrapService _clickWrapService;

        public ClickWrapController(IClickWrapService clickWrapService)
        {
            _clickWrapService = clickWrapService;
        }

        [HttpGet]
        public IActionResult Index([FromBody] RequestClickWrapModel model)
        {
            var response = _clickWrapService.CreateTimeTrackClickWrap(Context.Account.Id, model.WorkLogs);
            var responseBody = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);

            return Ok(responseBody);
        }
    }
}