using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

//Add the following namespaces
using Microsoft.AspNetCore.Authorization;
using WebTockenSampleProject.Models;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Options;

namespace WebTockenSampleProject.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationController : Controller
    {
        private readonly AppSettings _appSettings;
        public AuthorizationController(IOptions<AppSettings> appsettings)
        {
            _appSettings = appsettings.Value;
        }
        
        //Use this annotation to enable any user to call this endpoint
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<Response>> Authenticate(string user, string password)
        {
            //Make your logic to retrive the data from the database, and make sure to un salt and un hash the user name and password

            //Dumb comparisson
            if (password == user)
            {
                var TokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        //new Claim(ClaimTypes.Role, "genericRole"),
                        new Claim(ClaimTypes.Name, user)
                    }),
                    Expires = DateTime.UtcNow.AddHours(Convert.ToInt32(_appSettings.TokenHours)),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = TokenHandler.CreateToken(tokenDescriptor);
                var response = new
                {
                    Token = TokenHandler.WriteToken(token),
                    userMail = user
                };
                return new Response()
                {
                    message = "OK",
                    data = response
                };
            }
            else
            {
                return new Response()
                {
                    message = "Error",
                    data = ""
                };
            }

        }
    }
}