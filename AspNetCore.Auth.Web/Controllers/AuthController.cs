using AspNetCore.Auth.Web.Models;
using AspNetCore.Auth.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AspNetCore.Auth.Web.Controllers
{
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly IUserService _userService;
        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [Route("signin")]
        public IActionResult SignIn()
        {
            return View(new SignInModel());
        }

        [Route("signin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(SignInModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                User user;
                if (await _userService.ValidateCredentials(model.UserName, model.Password, out user))
                {
                    await SigninUser(user.UserName);
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(model);
        }

        [Route("signout")]
        [HttpPost]
        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [Route("signup")]
        public IActionResult SignUp()
        {
            return View(new SignUpModel());
        }

        [Route("signup")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(SignUpModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _userService.AddUser(model.Username, model.Password))
                {
                    await SigninUser(model.Username);
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("Error", "Could not add user. Username is already in use....");
            }
            return View(model);
        }

        //TODO: move to separate area/project
        /// <summary>
        /// To sign-in the user using cookie authentication
        /// </summary>
        /// <param name="userName">userName</param>
        /// <returns>void</returns>
        public async Task SigninUser(string userName)
        {
            // added claim
            var claims = new List<Claim>
            {
                // added claim for name identifier and name
                new Claim(ClaimTypes.NameIdentifier, userName),
                new Claim(ClaimTypes.GivenName, userName),
                new Claim("name", userName)
            };

            // setting the user claims and authentication scheme
            // we are passing only name claim and no role
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme, "name", null);
            var principle = new ClaimsPrincipal(identity);
            // initiate sign-in using the principle
            await HttpContext.SignInAsync(principle);
        }
    }
}