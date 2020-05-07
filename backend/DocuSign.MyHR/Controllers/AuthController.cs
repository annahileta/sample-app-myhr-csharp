using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IAuthenticationService = DocuSign.MyHR.Services.IAuthenticationService;

namespace DocuSign.MyHR.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService; 
        }

        [HttpPost]
        public IActionResult Login(string authType = "CodeGrant", string callbackUrl = "/ds/callback", string returnUrl = "/")
        {
            if (authType == "CodeGrant")
            {
                return LocalRedirect(_authenticationService.GetAuthorizationUrl(callbackUrl));
            }

            _authenticationService.AuthenticateFromJwt();
            
            return LocalRedirect(returnUrl);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("/api/auth/token")]
        public IActionResult GetToken(string code)
        {
            try
            {
                var token = _authenticationService.Authenticate(code);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
            catch (Exception ex)
            {
                BadRequest(ex.Message);
            }
            return BadRequest();
        }
    }
}