using System;
using System.Collections.Generic;
using System.Linq;
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
                        //Пока нету
                        return View("share____file", file);
                }
                else
                {
                    ViewBag.Folder = file;
                    ViewBag.RootSharingId = rootSharingId;
                    return View("SharedFolder", _fileManager.GetFolderContent(file.Id));
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
                if (file.IsFile)
                {
                    return File(file.Path, System.Net.Mime.MediaTypeNames.Application.Octet, file.Name);
                }
                else
                {
                    //TODO: Папку нужно заархивировать. Пока не пашет
                    return null;//File(file.Path, System.Net.Mime.MediaTypeNames.Application.Octet, file.Name + ".7z");
                }
            }
            else
            {
                throw new UnauthorizedAccessException();
            }

        }


    }
}