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
        [HttpPost]
        public async Task<ActionResult> Create(object obj, int? ParentId)
        {
            SystemFile _ParentElement = null;
            if (ParentId != null)
            {
                _ParentElement = _fileManeger.GetFileById((int)ParentId);
            }
            //достаем юзера, для дальнейшей работы
            string user_name = Request.GetOwinContext().Authentication.User.Identity.Name;
            AppUser user = await UserManager.FindByNameAsync(user_name);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found");
                return Redirect(Request.UrlReferrer.AbsoluteUri);
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
                var res = await _fileManeger.SaveSingleFile(new FileInfo(temp.FileName), user, _ParentElement, temp.ContentLength);
                temp.SaveAs(res);
            }
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> CreateFolder(string folderName, int? ParentId)
        {
            SystemFile _ParentElement = null;
            if (ParentId != null)
            {
                _ParentElement = _fileManeger.GetFileById((int)ParentId);
            }
            string user_name = Request.GetOwinContext().Authentication.User.Identity.Name;
            AppUser user = await UserManager.FindByNameAsync(user_name);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found");
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            }
            string path = await _fileManeger.CreateFolder(folderName, user, _ParentElement);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> Delete(int id)
        {
            if (await _fileManeger.DeleteSystemFile(id) == true)
            {
                ViewBag.Result = "Файл удален";
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            }
            ViewBag.Result = "Возникли ошибки";
            return Redirect(Request.UrlReferrer.AbsoluteUri);
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

            SystemFile folder = _fileManeger.GetFile(folderId);
            if (folder == null)
                throw new KeyNotFoundException();
            if (folder.Owner.UserName != HttpContext.User.Identity.Name)
                throw new UnauthorizedAccessException();

            ViewBag.Folder = folder;
            return View(_fileManeger.GetFolderContent(folderId));
        }

        //Расшариваем файл только для чтения
        [Authorize]
        public async Task<ActionResult> ShareReadOnly(int id)
        {
            ViewBag.Result = "Не получилось расшарить";
            if (await _fileManeger.Share((int)ShareType.ShareReadOnly, id) != String.Empty)
            {
                ViewBag.Result = "Расшарено успешно";
            }
            return View();
        }

        [Authorize]
        public FileResult Download(int? Id)
        {
            if (Id != null)
            {
                SystemFile file = _fileManeger.GetFile(Id);
                if (file.IsFile)
                    return File(file.Path, System.Net.Mime.MediaTypeNames.Application.Octet, file.Name);
                else
                    //TODO: Папку нужно заархивировать. Пока не пашет
                    return null;//File(file.Path, System.Net.Mime.MediaTypeNames.Application.Octet, file.Name + ".7z");
            }
            else throw new NullReferenceException();
        }
    }
}