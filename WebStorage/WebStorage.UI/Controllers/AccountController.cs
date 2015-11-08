using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using WebStorage.Domain.Entities;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using WebStorage.UI.Models;
using System.Web.Mvc;
using System.Linq;
using Facebook;

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
        // Инструмент работы с авторизацией
        private AppSignInManager SignInManager
        { get { return HttpContext.GetOwinContext().Get<AppSignInManager>(); } }

        [AllowAnonymous]
        public ActionResult Login(String returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel details)
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
                    await SignInManager.SignInAsync(_user, isPersistent: false, rememberBrowser: true);
                    return RedirectToAction("Index", "Home");
                }                
            }
            return View(details);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ExtLogin(string provider)
        {
            return new ChallengeResult(provider, Url.Action("ExtLoginCallback", 
                "Account", new { loginProvider = provider}));
        }

        [AllowAnonymous]
        public async Task<ActionResult> ExtLoginCallback()
        {

            var loginInfo = await AuthManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Перенаправляем пользователя на страницу с которой он начал если у него есть аккаунт
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToAction("Index", "Home");
                /*case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });*/
                case SignInStatus.Failure:
                default:
                    // Если у пользователя нет аккаунта просим создать его
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginViewModel { Email = loginInfo.Email });
            }
        }
        
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginViewModel model)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manager");
            }

            if (ModelState.IsValid)
            {
                var info = await AuthManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return RedirectToAction("Login");
                }
                var user = new AppUser { UserName = model.Name, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    user.CreateMainFolder();
                    await UserManager.UpdateAsync(user);
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Try another user name or email.");
                }                
            }            
            return View(model);
        }

        [Authorize]
        public ActionResult Logout()
        {
            //AuthManager.SignOut();
            SignInManager.AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        #region HttpUnauthorizedResult helper
        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUrl)
            {
                LoginProvider = provider;
                RedirectUrl = redirectUrl;
            }

            public string LoginProvider { get; set; }
            public string RedirectUrl { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                context.HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = RedirectUrl }, LoginProvider);
            }
        }
        #endregion
    } 
}