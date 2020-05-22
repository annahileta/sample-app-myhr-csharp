using DocuSign.MyHR.Domain;
using DocuSign.MyHR.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocuSign.MyHR.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Ok(_userService.GetUserDetails(Context.Account.Id, Context.User.Id));
        }

        [HttpPut]
        public IActionResult Index(UserDetails userDetails)
        {
            _userService.UpdateUserDetails(Context.Account.Id, Context.User.Id, userDetails);
            return Ok();
        }
    }
}
