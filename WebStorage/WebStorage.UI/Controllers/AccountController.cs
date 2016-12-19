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
using System.Data.Entity;

namespace WebStorage.UI.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        #region /////////////////////////////////// Properties ///////////////////////////////////
        
        private IAuthenticationManager AuthManager
        { get { return HttpContext.GetOwinContext().Authentication; } }
        
        private AppUserManager _userManager;
        private AppUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<AppUserManager>(); }
            set { _userManager = value; }
        }
        
        private AppSignInManager _signInManager;
        private AppSignInManager SignInManager
        {
            get { return _signInManager ?? HttpContext.GetOwinContext().Get<AppSignInManager>(); }
            set { _signInManager = value; }
        }
        #endregion

        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="signInManager"></param>
        public AccountController()
        {
            UserManager = null;
            SignInManager = null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="signInManager"></param>
        public AccountController(AppUserManager userManager, AppSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        #region ////////////////////////////////////// LogIn //////////////////////////////////////
        /// <summary>
        /// Login /Get/
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Login /Post/
        /// </summary>
        /// <param name="details"></param>
        /// <returns></returns>
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
                    TempData["loginmessage"] = "Может быть, у вас нет акаунта в Веб-хранилище. Пожалуйста, создайте его.";
                    ModelState.AddModelError("", "Неверный логин или пароль.");
                }
                else
                {
                    SignInStatus result = await SignInManager.PasswordSignInAsync(details.Name, details.Password, details.remember, shouldLockout: false);
                    if (result == SignInStatus.Success)
                    {
                        return RedirectToAction("Index", "File");
                    }
                    ModelState.AddModelError("", "Вы не имеете доступа.");
                }                
            }
            return View(details);
        }
        #endregion

        #region ////////////////////////////////// ExternalLogIn //////////////////////////////////
        /// <summary>
        /// ExtLogin
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        // Используем вспомогательный класс ChallengeResult
        public ActionResult ExtLogin(String provider)
        {
            return new ChallengeResult(provider, Url.Action("ExtLoginCallback", 
                "Account", new { loginProvider = provider}));
        }

        /// <summary>
        /// ExtLoginCallback
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        // Ф-я обратного вызова для входа на сайт(когда пытаемся достучатся с пом соц сетей) 
        public async Task<ActionResult> ExtLoginCallback()
        {

            var loginInfo = await AuthManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }            
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    // Перенаправляем пользователя на стартовую страницу проекта если у него есть аккаунт
                    return RedirectToAction("Index", "File");
                case SignInStatus.Failure:
                default:
                    // Если у пользователя нет аккаунта просим создать его
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginViewModel { Email = loginInfo.Email, Name = loginInfo.DefaultUserName });
            }
        }
        /// <summary>
        /// ExternalLoginConfirmation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        /*
        Некоторые соц сети(twitter) не дают информацию пользователя в виде его email.
        У Facebook можно выудить это с помощью его API, но потом вылазит исключение связанное с @Html.AntiForgeryToken() в представлении.
        Я нашел вариант исправить это, но не уверен, что оно будет обеспечивать безопасность приложения, поэтому будем просить пользователя
        ввести почту и имя. Если будет возможность эту информацию будем вытягивать из  ExternalLoginInfo.Email и ExternalLoginInfo.DefaultUserName.
        */
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                ExternalLoginInfo info = await AuthManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return RedirectToAction("Login");
                }

                AppUser user = new AppUser { UserName = model.Name, Email = model.Email };
                IdentityResult result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    user.CreateMainFolder();
                    await UserManager.UpdateAsync(user);
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToAction("Index", "File");
                    }
                    AddErrors(result);
                }
                else
                {
                    AddErrors(result);
                }                
            }            
            return View(model);
        }
        #endregion

        #region /////////////////////////////////// Logout(off) ///////////////////////////////////
        /// <summary>
        /// Logout
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Logout()
        {
            //SignInManager.AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie); // или
            AuthManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Account");
        }
        #endregion
        
        #region/////////////////////////// HttpUnauthorizedResult helper //////////////////////////
        /// <summary>
        /// Вспомогательный класс для реализации авторизации с помощью провайдеров соц сетей.
        /// </summary>
        private class ChallengeResult : HttpUnauthorizedResult
        {
            /// <summary>
            /// Ctor that takes 2 arguments
            /// </summary>
            /// <param name="provider"></param>
            /// <param name="redirectUrl"></param>
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

        #region ///////////////////////////////////// Dispose //////////////////////////////////////
        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }
        #endregion

        /// <summary>
        /// Add error to ModelState
        /// </summary>
        /// <param name="result"></param>
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}
////////////////////////////////////////////// END ///////////////////////////////////////////