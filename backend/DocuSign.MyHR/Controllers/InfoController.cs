using Microsoft.AspNetCore.Mvc;

namespace DocuSign.MyHR.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InfoController : ControllerBase
    {
        [HttpGet]
        [Route("Ping")]
        public string Ping()
        {
            return "OK";
        }
    }
}
