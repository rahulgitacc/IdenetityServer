using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.Auth.Web.Controllers
{
    [Route("auth")]
    public class AuthController : Controller
    {
        [Route("signin")]
        public IActionResult SignIn()
        {
            // system will automatically detect that we will use FacebookDefaults.AuthenticationScheme to authenticate
            // As we have configured that in startup
            // configure after authentication by passing AuthenticationProperties instance
            return Challenge(new AuthenticationProperties { RedirectUri = "/" });
        }
    }
}