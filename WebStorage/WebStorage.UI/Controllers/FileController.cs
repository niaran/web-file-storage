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
using WebStorage.UI.Models;

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
            //Проверяем текущего юзера на владение папки или файла.
            string user_name = Request.GetOwinContext().Authentication.User.Identity.Name;
            AppUser user = await UserManager.FindByNameAsync(user_name);
            SystemFile file = _fileManeger.GetFile(id);
            if (user == null || file.OwnerId != user.Id || file == null)
            {
                return null;
            }
            //Удаляем папку
            if (await _fileManeger.DeleteSystemFile(id) == true)
            {
                ViewBag.Result = "Файл удален";
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            }
            ViewBag.Result = "Возникли ошибки";
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> Info(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            //Проверяем текущего юзера на владение папки или файла.
            string user_name = Request.GetOwinContext().Authentication.User.Identity.Name;
            AppUser user = await UserManager.FindByNameAsync(user_name);
            SystemFile file = _fileManeger.GetFile(id);

            if (user == null || file == null || file.OwnerId != user.Id)
            {
                return null;
            }
            return View(file);
        }


        [Authorize]
        public async Task<ActionResult> Index(int? folderId)
        {
            if (Request.Cookies["OrderBy"] != null)
            {
                _fileManeger.OrderValue = int.Parse(Request.Cookies["OrderBy"].Value);
            }
            string user_name = Request.GetOwinContext().Authentication.User.Identity.Name;
            AppUser user = await UserManager.FindByNameAsync(user_name);

            //Если папка не передана, то выведет корень - все папки, к которым имеет доступ текущий юзер
            if (folderId == null)
            {
                ViewBag.Folder = null;
                return View(_fileManeger.GetFolderContentWithUser(null, user));
            }
            SystemFile folder = _fileManeger.GetFile(folderId);
            if (folder == null)
                throw new KeyNotFoundException();
            if (folder.Owner.UserName != HttpContext.User.Identity.Name)
                throw new UnauthorizedAccessException();
            ViewBag.Folder = folder;
            ViewBag.UserID = UserManager.FindByName(HttpContext.User.Identity.Name);
            return View(_fileManeger.GetFolderContentWithUser(folderId, user));
        }

        //Расшариваем файл 
        [Authorize]
        public async Task<ActionResult> ShareReadOnly(int id)
        {
            SystemFile file = _fileManeger.GetFile(id);
            //Проверяем текущего юзера на владение папки.
            string user_name = Request.GetOwinContext().Authentication.User.Identity.Name;
            AppUser user = await UserManager.FindByNameAsync(user_name);
            if (user == null || file.OwnerId != user.Id || file == null)
            {
                return null;
            }

            if (await _fileManeger.Share((int)ShareType.ShareReadOnly, id) != null)
            {
                ViewBag.Result = "Расшарено успешно";
            }
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        //Аншарим файл 
        [Authorize]
        public async Task<ActionResult> ShareOwnerOnly(int id)
        {
            SystemFile file = _fileManeger.GetFile(id);
            //Проверяем текущего юзера на владение папки.
            string user_name = Request.GetOwinContext().Authentication.User.Identity.Name;
            AppUser user = await UserManager.FindByNameAsync(user_name);
            if (user == null || file.OwnerId != user.Id || file == null)
            {
                return null;
            }

            if (await _fileManeger.Share((int)ShareType.OwnerOnly, id) == null)
            {
                ViewBag.Result = "Аншарен успешно";
            }
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }
        [Authorize]
        public async Task<ActionResult> SharedList()
        {
            ViewBag.Result = "У вас нет рассшаренных файлов  папок";
            //достаем юзера, для дальнейшей работы
            string user_name = Request.GetOwinContext().Authentication.User.Identity.Name;
            AppUser user = await UserManager.FindByNameAsync(user_name);
            if (user != null)
            {
                var sharedList = _fileManeger.GetUserSharedFiles(user);
                return View(sharedList);
            }
            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<FileResult> Download(int? Id)
        {
            if (Id != null)
            {
                SystemFile file = _fileManeger.GetFile(Id);
                //Проверяем текущего юзера на владение папки.
                string user_name = Request.GetOwinContext().Authentication.User.Identity.Name;
                AppUser user = await UserManager.FindByNameAsync(user_name);
                if (user == null || file.OwnerId != user.Id)
                {
                    return null;
                }

                if (file.Size > 104857600)
                    return null;

                if (file.IsFile)
                    return File(file.Path, System.Net.Mime.MediaTypeNames.Application.Octet, file.Name + file.Format);
                else
                {
                    string path = _fileManeger.ArchiveTheFolder(file, true);
                    var bytes = System.IO.File.ReadAllBytes(path);
                    System.IO.File.Delete(path);
                    return File(bytes, System.Net.Mime.MediaTypeNames.Application.Octet, file.Name + ".zip"); ;
                }
            }
            else throw new NullReferenceException();
        }

        [Authorize]
        public ActionResult OrderList(int orderBy)
        {
            var userCookie = new HttpCookie("OrderBy", orderBy.ToString());
            HttpContext.Response.Cookies.Add(userCookie);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> SearchFiles(string searchString, string RootSharingId, int? ParentId)
        {
            ViewBag.RootSharingId = RootSharingId;
            //Проверяем текущего юзера.
            string user_name = Request.GetOwinContext().Authentication.User.Identity.Name;
            AppUser user = await UserManager.FindByNameAsync(user_name);
            if(user == null)
            {
                ViewBag.Auth = "noUser";
            }

            if (user == null && ParentId == null && RootSharingId == null)
            {
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            }
            else if (ParentId != null)
            {
                ViewBag.SearchString = searchString;
                return View(_fileManeger.SearchFileInParentFolder(searchString, (int)ParentId));
            }
            ViewBag.SearchString = searchString;
            return View(_fileManeger.SearchFileForUser(searchString, user));
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> EditFileName(string fileName, int fileId)
        {
            SystemFile file = _fileManeger.GetFile(fileId);
            //Проверяем текущего юзера на владение папки.
            string user_name = Request.GetOwinContext().Authentication.User.Identity.Name;
            AppUser user = await UserManager.FindByNameAsync(user_name);
            if (user == null ||file == null || file.OwnerId != user.Id)
            {
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            }
            await _fileManeger.EditFileName(file, fileName);
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

#region ///////////////////// .doc /////////////////////

        /// <summary>
        /// Create .doc file
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ParentId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> CreateDocFile(String name, Int32? ParentId)
        {
            WebStorageDoc model;
            try
            {
                model = await WorkWithDocFile(name, ParentId, String.Empty);
            }
            catch
            {
                return View("EditError", null,
                    "Have some problem with file. Please close the tab and try again later.");
            }            
            return View("EditDocFile", model);
        }

        /// <summary>
        /// Edit .doc file.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult EditDocFile(Int32 Id)
        {
            SystemFile file = _fileManeger.GetFileById(Id);
            WebStorageDoc model = new WebStorageDoc() { FileName = file.Name, ParentId = file.ParentId };
            try
            {
                model.EditorContent = model.ReadDocFile(file.Path);
            }
            catch
            {
                return View("EditError", null,
                    "Have some problem with file. Please close the tab and try again later.");
            }

            return View(model);
        }

        /// <summary>
        /// Edit .doc file. HttpPost
        /// Update existing  .doc file.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> EditDocFile(WebStorageDoc model)
        {
            WebStorageDoc new_model;
            try
            {
                new_model = await WorkWithDocFile(model.FileName, model.ParentId, model.EditorContent);
            }
            catch
            {
                return View("EditError", null, 
                    "Have some problem with file. Please close the tab and try again later.");
            }
            return View(new_model);
        }

        /// <summary>
        /// Get parent folder and map .doc view model
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ParentId"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        private async Task<WebStorageDoc> WorkWithDocFile(String name, Int32? ParentId, String content)
        {
            ////////////// Find user //////////////
            String user_name = Request.GetOwinContext().Authentication.User.Identity.Name;
            AppUser user = UserManager.FindByName(user_name);
            ////////////// Additional Info instances //////////////
            String _pathToParentFolder;
            SystemFile ParentFolder;
            ////////////// Get additional Info //////////////           
            if (!ParentId.HasValue)
            {
                _pathToParentFolder = user.PathToMainFolder;
                ParentFolder = null;
            }
            else
            {
                _pathToParentFolder = _fileManeger.GetFileById(ParentId.Value).Path;
                ParentFolder = _fileManeger.GetFileById(ParentId.Value);
            }
            ////////////// Get .doc view model ////////////// 
            WebStorageDoc model = await MappingDocViewModel(user, content, ParentId, ParentFolder, _pathToParentFolder, name);
            
            return model;
        }

        /// <summary>
        /// Update or create .doc file.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="editorContent"></param>
        /// <param name="fileParentId"></param>
        /// <param name="fileParentFolder"></param>
        /// <param name="_pathToParentFolder"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private async Task<WebStorageDoc> MappingDocViewModel(AppUser user, String editorContent, Int32? fileParentId, SystemFile fileParentFolder, 
            String _pathToParentFolder, String fileName)
        {
            WebStorageDoc model = new WebStorageDoc();
            SystemFile _file = _fileManeger.GetFolderContent(fileParentId).ToList<SystemFile>().FirstOrDefault(o => o.Name == fileName && o.Format == ".boxdoc");
            Decimal fileSizeBeforUpdate = 0M;
            ////////////// Fill ViewModel //////////////
            if (_file == null)
            {
                ////////////// Create temp file //////////////
                System.IO.File.Create(Path.Combine(_pathToParentFolder, fileName + ".boxdoc")).Close();
                FileInfo _info = null;
                DirectoryInfo _di = new DirectoryInfo(_pathToParentFolder);
                ////////////// Get info about temp file //////////////
                foreach (FileInfo _finfo in _di.GetFiles("*.boxdoc"))
                {
                    if (_finfo.Name == fileName + ".boxdoc")
                    {
                        _info = _finfo;
                    }
                }
                ////////////// Create new path for temp file //////////////
                String savepath = await _fileManeger.SaveSingleFile(_info, user, fileParentFolder, _info.Length);
                ////////////// Fill ViewModel //////////////
                model.WriteToDocFile(savepath, editorContent);
                model.EditorContent = model.ReadDocFile(savepath);
                model.ParentId = fileParentId;
                model.FileName = fileName;
                ////////////// Delete temp file //////////////
                System.IO.File.Delete(_info.FullName);
            }
            else
            {
                ////////////// Fill ViewModel ////////////////////////
                //============ Get additional info about file ========
                fileSizeBeforUpdate = new FileInfo(_file.Path).Length;                
                //============ Write to file =========================
                model.WriteToDocFile(_file.Path, editorContent);
                //============ Change file info ======================
                _file.Size = new FileInfo(_file.Path).Length;
                if (fileParentId != null)
                {
                    fileParentFolder.Size -= fileSizeBeforUpdate;
                    fileParentFolder.Size += _file.Size;
                } 
                await _fileManeger.dbContext.SaveChangesAsync();
                //============ Fill ViewModel ========================
                model.EditorContent = model.ReadDocFile(_file.Path);
                model.ParentId = fileParentId;
                model.FileName = fileName;                
            }
            return model;
        }
        #endregion
    }
}