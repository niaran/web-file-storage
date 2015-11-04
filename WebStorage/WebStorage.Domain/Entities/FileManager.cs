using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebStorage.Domain.Abstract;
using Microsoft.Owin;
using WebStorage.Domain.Concrete;

namespace WebStorage.Domain.Entities
{
    // Класс для работы с файлами (удалять, добавлять, переименовывать и тд)
    public class FileManager : IFileManager
    {
        public AppDbContext dbContext;
        public FileManager()
        {
            dbContext = AppDbContext.Create();
        }

        public FileManager(AppDbContext db)
        {
            dbContext = db;
        }
        public async Task<string> SaveSingleFile(FileInfo fileInfo, AppUser user, SystemFile ParentElement, decimal fileSize)
        {
            //Создаем путь.
            string path = String.Empty;
            //В случае, если корневой каталог, путь в корень папки юзера
            if (ParentElement == null)
            {
                path = user.PathToMainFolder + "\\" + fileInfo.Name;
            }
            //В случае, если имеется родительский элемент
            else
            {
                path = ParentElement.Path + "\\" + fileInfo.Name;
            }

            //Создаем и заполняем обьект файла
            SystemFile sysFile = new SystemFile();
            sysFile.Name = fileInfo.Name;
            sysFile.Uploaded = DateTime.Now;
            sysFile.OwnerId = user.Id;
            sysFile.Path = path;
            sysFile.Size = fileSize;
            sysFile.Format = fileInfo.Extension;
            sysFile.IsFile = true;
            sysFile.ParentFolder = ParentElement;
            if (ParentElement != null)
            {
                sysFile.ParentId = ParentElement.Id;
            }

            //Сохраняем файл в БД
            dbContext.SystemFiles.Add(sysFile);
            await dbContext.SaveChangesAsync();

            //Возвращаем путь к нему, для заливки в файловую систему
            return path;
        }
    }
}
