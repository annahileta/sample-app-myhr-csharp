using System;
using System.ComponentModel.DataAnnotations;
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

        [HttpGet]
        public IActionResult Login(
            [Required(AllowEmptyStrings = false)]string authType, 
            [Required(AllowEmptyStrings = false)]string callbackUrl, 
            string returnUrl ="/")
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (authType == "CodeGrant")
            {
                return LocalRedirect(_authenticationService.GetAuthorizationUrl(callbackUrl));
            }

            if (authType == "JWT")
            {
                _authenticationService.AuthenticateFromJwt();

                return LocalRedirect(returnUrl);
            }

            return BadRequest("Authentication type is not supported"); 
        }
         
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
                return BadRequest(ex.Message);
            }
        }
    }
}