using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DocuSign.MyHR.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        public UserController()
        {

        }

        [HttpGet]
        [Route("user")]
        public string GetUser()
        {
            return JsonConvert.SerializeObject(Context.User);
        }
    }
}
