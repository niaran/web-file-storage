using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebStorage.Domain.Concrete;
using WebStorage.Domain.Entities;

namespace WebStorage.UI.Controllers
{
    public class FileController : Controller
    {
        FileManager manager = new FileManager();

        [Authorize]
        public ActionResult Index(int? folderId)
        {
            //Если папка не передана, то выведет корень - все папки, к которым имеет доступ текущий юзер
            if (folderId == null)
            {
                ViewBag.Folder = null;
                return View(manager.dbContext.SystemFiles.Where(x => x.Owner.UserName == HttpContext.User.Identity.Name && x.ParentFolder == null));
            } 
            ViewBag.Folder = manager.GetFile(folderId);
            return View(manager.GetFolderContent(folderId));
        }
    }
}