using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AppName.Web.DataStructures;
using AppName.Web.Extensions;
using AppName.Web.Models;
using AppName.Web.Providers;
using AppName.Web.Repositories;
using AppName.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace AppName.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IDotNetProvider _dotNetProvider;

        public AccountController(IUserRepository userRepository,
            IDotNetProvider dotNetProvider)
        {
            _userRepository = userRepository;
            _dotNetProvider = dotNetProvider;
        }

        [Route("/auth/success")]
        public async Task LoginSuccess(string returnUrl = "/")
        {
            var existingUser = await _userRepository.GetUserByUserIdAsync(HttpContext.User.UserId());
            
            var user = existingUser ?? new AppUser(HttpContext.User.UserId(), _dotNetProvider.DateTimeNow);

            foreach (var claim in HttpContext.User.Claims)
            {
                switch (claim.Type)
                {
                    case "picture":
                        user.PictureUri = claim.Value;
                        break;
                    case "name":
                        user.Name = claim.Value;
                        break;
                }
            }
            
            await _userRepository.UpsertUserAsync(user);
            
            Response.Redirect(returnUrl);
        }
        
        [AllowAnonymous]
        public async Task Login(string returnUrl = "/")
        {
            string uri = $"/auth/success?returnUrl={returnUrl}";
            await HttpContext.ChallengeAsync("Auth0", new AuthenticationProperties { RedirectUri = uri });
        }
        
        public async Task<IActionResult> Tokens()
        {
            // If the user is authenticated, then this is how you can get the access_token and id_token
            if (User.Identity.IsAuthenticated)
            {
                string accessToken = await HttpContext.GetTokenAsync("access_token");
                string idToken = await HttpContext.GetTokenAsync("id_token");

                ViewData["access_token"] = accessToken;
                ViewData["id_token"] = idToken;
                

                // Now you can use them. For more info on when and how to use the 
                // access_token and id_token, see https://auth0.com/docs/tokens
            }

            return View();
        }

        public async Task Logout()
        {
            await HttpContext.SignOutAsync("Auth0", new AuthenticationProperties
            {
                // Indicate here where Auth0 should redirect the user after a logout.
                // Note that the resulting absolute Uri must be whitelisted in the 
                // **Allowed Logout URLs** settings for the client.
                RedirectUri = Url.Action("Index", "Home")
            });
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public IActionResult Claims()
        {
            ViewBag.Title = "View claims";

            return View(HttpContext.User.Claims);
        }
    }
}