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
    public class ManagerController : Controller
    {
        #region /////////////////////////////////// Properties ///////////////////////////////////

        private AppUserManager UserManager
        {
            get { return HttpContext.GetOwinContext().GetUserManager<AppUserManager>(); }
        }
        #endregion

        /*
        Этот метод действия представлен в качестве тестового.
        На самом деле в дальнейшем такого не будет.

        Также необходимо будет исправить в дальнейшем некоторые 
        перенаправления к представлениям.
        */
        public ActionResult Index()
        {
            return View(UserManager.Users);
        }

        #region /////////////////////////////// Create user section ///////////////////////////////

        /// <summary>
        /// Create /Get/
        /// </summary>
        /// <returns></returns>
        // Создать пользователя
        public ActionResult Create()
        {
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
            if (ModelState.IsValid)
            {
                // создаем переменную пользователя типа AppUser
                AppUser _user = new AppUser() { UserName = user.Name, Email = user.Email };
                // Сохраняем в базу
                IdentityResult _result = await UserManager.CreateAsync(_user, user.Password);
                if (_result.Succeeded)
                {
                    // Создаем корневую директорию пользователя
                    _user.CreateMainFolder();
                    // Сохраняем это
                    IdentityResult _res = await UserManager.UpdateAsync(_user);
                    if (!_res.Succeeded)
                    {
                        return View(user);
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    AddErrors(_result);
                }
            }
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

        #region //////////////////////////////// Edit user section ////////////////////////////////
        /// <summary>
        /// Edit user info /Get/
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Edit user info /Post/
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        //[ValidateAntiForgeryToken] ???????????????????????
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