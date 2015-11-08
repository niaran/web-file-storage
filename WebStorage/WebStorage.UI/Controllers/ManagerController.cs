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
    public class ManagerController : Controller
    {
        public ActionResult Index()
        {
            return View(UserManager.Users);
        }

        private AppUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }

        #region create section
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(UserViewModel user)
        {
            if (ModelState.IsValid)
            {
                AppUser _user = new AppUser() { UserName = user.Name, Email = user.Email };
                IdentityResult _result = await UserManager.CreateAsync(_user, user.Password);
                if (_result.Succeeded)
                {
                    _user.CreateMainFolder();
                    IdentityResult _res = await UserManager.UpdateAsync(_user);
                    if (!_res.Succeeded)
                    {
                        return View(user);
                    }
                    AppUser User = await UserManager.FindAsync(user.Name, user.Password);
                    // генерируем токен для подтверждения регистрации
                    var code = await UserManager.GenerateEmailConfirmationTokenAsync(User.Id);
                    // создаем ссылку для подтверждения
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = User.Id, code = code },
                               protocol: Request.Url.Scheme);
                    // отправка письма
                    await UserManager.SendEmailAsync(User.Id, "Подтверждение электронной почты",
                               "Для завершения регистрации перейдите по ссылке:: <a href=\""
                                                               + callbackUrl + "\">завершить регистрацию</a>");

                    return View("DisplayEmail");

                    //return RedirectToAction("Index");
                }
                else
                {
                    AddErrors(_result);
                }
            }
            return View(user);
        }
        #endregion

        #region delete section
        [HttpPost]
        public async Task<ActionResult> Delete(String Id)
        {
            AppUser _user = await UserManager.FindByIdAsync(Id);
            if (_user != null)
            {
                IdentityResult _result = await UserManager.DeleteAsync(_user);
                if (_result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return View("Error", _result.Errors);
                }
            }
            else
            {
                return View("Error", new string[] { "User Not Found" });
            }
        }
        #endregion

        #region edit section
        public async Task<ActionResult> Edit(String Id)
        {
            AppUser _user = await UserManager.FindByIdAsync(Id);
            if (_user != null)
            {
                return View(_user);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Edit(String Id, String email, String password)
        {
            AppUser _user = await UserManager.FindByIdAsync(Id);
            if (_user != null)
            {
                // valid email 
                _user.Email = email;
                IdentityResult validMail = await UserManager.UserValidator.ValidateAsync(_user);
                if (!validMail.Succeeded)
                {
                    AddErrors(validMail);
                }

                // valid password
                IdentityResult validPass = null;
                if (password != String.Empty)
                {
                    validPass = await UserManager.PasswordValidator.ValidateAsync(password);
                    if (validPass.Succeeded)
                    {
                        _user.PasswordHash = UserManager.PasswordHasher.HashPassword(password);
                    }
                    else
                    {
                        AddErrors(validPass);
                    }
                }

                // update user
                if ((validMail.Succeeded && validPass == null) || (validMail.Succeeded && password != string.Empty && validPass.Succeeded))
                {
                    IdentityResult _result = await UserManager.UpdateAsync(_user);
                    if (_result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        AddErrors(_result);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "User not found");
            }
            return View(_user);
        }
        #endregion

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}