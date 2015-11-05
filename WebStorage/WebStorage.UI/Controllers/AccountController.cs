using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security;
using Owin;
using WebStorage.Domain.Entities;
using WebStorage.Domain.Concrete;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using WebStorage.UI.Models;
using System.Web.Mvc;

namespace WebStorage.UI.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        // Инструмент работы с авторизацией
        private IAuthenticationManager AuthManager
        { get { return HttpContext.GetOwinContext().Authentication; } }
        // Инструмент работы с пользователями
        private AppUserManager UserManager
        { get { return HttpContext.GetOwinContext().GetUserManager<AppUserManager>(); } }

        [AllowAnonymous]
        public ActionResult Login(String returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel details, String returnUrl)
        {
            if (ModelState.IsValid)
            {
                AppUser _user = await UserManager.FindAsync(details.Name, details.Password);

                if (_user == null)
                {
                    TempData["loginmessage"] = "Maybe you dont have web storagr account. Please, create it.";
                    ModelState.AddModelError("", "Invalid name or password.");
                }
                else
                {
                    ClaimsIdentity ident = await UserManager.CreateIdentityAsync(_user,
                        DefaultAuthenticationTypes.ApplicationCookie);
                    AuthManager.SignOut();
                    AuthManager.SignIn(new AuthenticationProperties()
                    {
                        IsPersistent = false
                    }, ident);
                    return Redirect(returnUrl);
                }                
            }
            return View(details);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult GoogleLogin(string returnUrl)
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleLoginCallback", new { returnUrl = returnUrl })
            };
            // перенаправляем пользователя на страницу авторизации Google
            HttpContext.GetOwinContext().Authentication.Challenge(properties, "Google");
            return new HttpUnauthorizedResult();
        }

        public async Task<ActionResult> GoogleLoginCallback(string returnUrl)
        {
            ExternalLoginInfo loginInfo = await AuthManager.GetExternalLoginInfoAsync();
            AppUser _user = await UserManager.FindAsync(loginInfo.Login);
            if (_user == null)
            {
                _user = new AppUser
                {
                    UserName = loginInfo.DefaultUserName,
                    Email = loginInfo.Email
                };

                IdentityResult _result = await UserManager.CreateAsync(_user);
                if (!_result.Succeeded)
                {
                    return View("Error", _result.Errors);
                }
                else
                {
                    _result = await UserManager.AddLoginAsync(_user.Id, loginInfo.Login);
                    if (!_result.Succeeded)
                    {
                        return View("Error", _result.Errors);
                    }
                }
            }

            ClaimsIdentity identity = await UserManager.CreateIdentityAsync(_user, 
                DefaultAuthenticationTypes.ApplicationCookie);
            identity.AddClaims(loginInfo.ExternalIdentity.Claims);
            AuthManager.SignIn(new AuthenticationProperties
            {
                IsPersistent = false
            }, identity);
            return Redirect(returnUrl ?? "/");
        }

        [Authorize]
        public ActionResult Logout()
        {
            AuthManager.SignOut();
            return RedirectToAction("Index", "Home");
        }
    } 
}