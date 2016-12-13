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
using System.IO;
using reCaptcha;
using System.Configuration;

namespace WebStorage.UI.Controllers
{
    [Authorize]
    public class ManagerController : Controller
    {
        #region /////////////////////////////////// Properties ///////////////////////////////////

        private AppUserManager UserManager
        {
            get { return HttpContext.GetOwinContext().GetUserManager<AppUserManager>(); }
        }

        private FileManager fileManager;
        #endregion

        public ManagerController()
        {
            fileManager = new FileManager();
        }

        #region /////////////////////////////// Create user section ///////////////////////////////

        /// <summary>
        /// Create /Get/
        /// </summary>
        /// <returns></returns>
        // Создать пользователя
        [AllowAnonymous]
        public ActionResult Create()
        {
            ViewBag.Recaptcha = ReCaptcha.GetHtml(ConfigurationManager.AppSettings["ReCaptcha:SiteKey"]);
            ViewBag.publicKey = ConfigurationManager.AppSettings["ReCaptcha:SiteKey"];
            return View();
        }

        /// <summary>
        /// Create /Post/
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Create(UserViewModel user)
        {
            if (ModelState.IsValid && ReCaptcha.Validate(ConfigurationManager.AppSettings["ReCaptcha:SecretKey"]))
            {
                // создаем переменную типа AppUser
                AppUser _user = new AppUser() { UserName = user.Name, Email = user.Email };
                // Сохраняем в базу
                IdentityResult _result = await UserManager.CreateAsync(_user, user.Password);
                if (_result.Succeeded)
                {
                    // Создаем корневую директорию пользователя
                    _user.CreateMainFolder();
                    // Сохраняем это
                    IdentityResult _res = await UserManager.UpdateAsync(_user);
                    return RedirectToAction("Index", "File");
                }
                else
                {
                    AddErrors(_result);
                }
            }

            ViewBag.RecaptchaLastErrors = ReCaptcha.GetLastErrors(this.HttpContext);
            ViewBag.publicKey = ConfigurationManager.AppSettings["ReCaptcha:SiteKey"];
            return View(user);
        }
        #endregion

        #region /////////////////////////////// Delete user section /////////////////////////////// 
        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(String Id)
        {
            AppUser _user = await UserManager.FindByIdAsync(Id);
            if (_user != null)
            {
                //IdentityResult _result = await UserManager.DeleteAsync(_user);
                try
                {
                    IEnumerable<SystemFile> allUserFiles = fileManager.dbContext.SystemFiles.Where(o => o.OwnerId == _user.Id).AsEnumerable<SystemFile>();

                    fileManager.dbContext.SystemFiles.RemoveRange(allUserFiles);
                    fileManager.dbContext.SaveChanges();

                    foreach (var item in new DirectoryInfo(_user.PathToMainFolder).EnumerateFiles())
                    {
                        item.Delete();
                    }
                    foreach (var item in new DirectoryInfo(_user.PathToMainFolder).EnumerateDirectories())
                    {
                        item.Delete(true);
                    }
                    return RedirectToAction("Login", "Account");
                }
                catch
                {
                    return View("Error", "Have some error");
                }
            }
            else
            {
                return View("Error", new string[] { "User Not Found" });
            }
        }
        #endregion

        #region //////////////////////////////// Edit user section ////////////////////////////////
        /// <summary>
        /// Edit user info /Get/
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<ActionResult> Edit()
        {
            AppUser _user = await UserManager.FindByNameAsync(HttpContext.User.Identity.Name);

            if (_user != null)
            {
                return View(_user);
            }
            else
            {
                return RedirectToAction("Index", "File");
            }
        }

        /// <summary>
        /// Edit user info /Post/
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
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
                        return RedirectToAction("Index", "File");
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

        /// <summary>
        /// Add errors to ModelState.AddModelError
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