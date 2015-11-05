using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebStorage.Domain.Concrete;
using WebStorage.Domain.Entities;

namespace WebStorage.UI.Controllers
{
    public class FileController : Controller
    {
        private AppUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }

        //Класс для работы с файлам в БД.
        private FileManager _fileManeger;
        public FileController()
        {
            _fileManeger = new FileManager();
        }


        [Authorize]
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Create(object obj)
        {
            //достаем юзера, для дальнейшей работы
            string user_name = Request.GetOwinContext().Authentication.User.Identity.Name;
            AppUser user = await UserManager.FindByNameAsync(user_name);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found");
                return View();
            }

            //достаем файлы из пост-запроса
            HttpFileCollectionBase Files = Request.Files;
            for (int i = 0; i < Files.Count; i++)
            {
                HttpPostedFileBase temp = Files[i];
                if (temp == null || temp.ContentLength == 0)
                {
                    continue;
                }
                var res = await _fileManeger.SaveSingleFile(new FileInfo(temp.FileName), user, null, temp.ContentLength);
                temp.SaveAs(res);
            }
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult CreateFolder()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> CreateFolder(string folderName)
        {
            string user_name = Request.GetOwinContext().Authentication.User.Identity.Name;
            AppUser user = await UserManager.FindByNameAsync(user_name);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found");
                return View();
            }
            string path = await _fileManeger.CreateFolder(folderName, user, null);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return View();
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> Delete(int id)
        {
            if (await _fileManeger.DeleteSystemFile(id) == true)
            {
                ViewBag.Result = "Файл удален";
                return View();
            }
            ViewBag.Result = "Возникли ошибки";
            return View();
        }

	[Authorize]
        public ActionResult Index(int? folderId)
        {
            //Если папка не передана, то выведет корень - все папки, к которым имеет доступ текущий юзер
            if (folderId == null)
            {
                ViewBag.Folder = null;
                return View(_fileManeger.dbContext.SystemFiles.Where(x => x.Owner.UserName == HttpContext.User.Identity.Name && x.ParentFolder == null));
            } 
            ViewBag.Folder = _fileManeger.GetFile(folderId);
            return View(_fileManeger.GetFolderContent(folderId));
        }

    }
}