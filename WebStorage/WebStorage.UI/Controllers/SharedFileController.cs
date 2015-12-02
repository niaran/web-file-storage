using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebStorage.Domain.Entities;

namespace WebStorage.UI.Controllers
{
    public class SharedFileController : Controller
    {
        private FileManager _fileManager;
        public SharedFileController()
        {
            _fileManager = new FileManager();
        }

        [AllowAnonymous]
        public ActionResult Index(string rootSharingId, int? contentId)
        {
            SystemFile file = _fileManager.AccessSharedFile(rootSharingId, contentId);

            if (file != null)
            {
                if (file.IsFile)
                {
                    if (contentId != null)
                        throw new UnauthorizedAccessException();
                    else
                    {
                        ViewBag.Root = rootSharingId;
                        return View("SharedInfo", file);
                    }
                }
                else
                {
                    ViewBag.Folder = file;
                    ViewBag.RootSharingId = rootSharingId;
                    return View("SharedFolder", _fileManager.GetFolderContent(file.Id).Where(x => x.Sharing_Atribute != 1));
                }
            }
            else
                throw new UnauthorizedAccessException();

        }

        [AllowAnonymous]
        public FileResult Download(string rootSharingId, int? contentId)
        {
            SystemFile file = _fileManager.AccessSharedFile(rootSharingId, contentId);
            if (file != null)
            {
                if (file.Size > 104857600)
                    return null;
                if (file.IsFile)
                {
                    return File(file.Path, System.Net.Mime.MediaTypeNames.Application.Octet, file.Name);
                }
                else
                {
                    string path = _fileManager.ArchiveTheFolder(file, false);
                    var bytes = System.IO.File.ReadAllBytes(path);
                    System.IO.File.Delete(path);
                    return File(bytes, System.Net.Mime.MediaTypeNames.Application.Octet, file.Name + ".zip"); ;
                }
            }
            else
            {
                throw new UnauthorizedAccessException();
            }

        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Info(string rootSharingId, int? contentId)
        {
            SystemFile file = _fileManager.AccessSharedFile(rootSharingId, contentId);

            if (file != null)
            {
                ViewBag.Root = rootSharingId;
                return View("SharedInfo", file);
            }
            else
                throw new UnauthorizedAccessException();
            
        }


    }
}