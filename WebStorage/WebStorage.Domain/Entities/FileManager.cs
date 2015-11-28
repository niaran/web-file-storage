using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebStorage.Domain.Abstract;
using Microsoft.Owin;
using WebStorage.Domain.Concrete;
using System.IO;
using System.IO.Compression;

namespace WebStorage.Domain.Entities
{

    // Класс для работы с файлами (удалять, добавлять, переименовывать и тд)
    public class FileManager : IFileManager
    {
        public AppDbContext dbContext;
        public int OrderValue { get; set; }

        public FileManager()
        {
            dbContext = AppDbContext.Create();
            OrderValue = (int)OrderType.ByDate;
        }

        public FileManager(AppDbContext db)
        {
            dbContext = db;
        }
        //Метод для вписания файла в БД и получения его физического пути, для дальнейшего использования
        public async Task<string> SaveSingleFile(FileInfo fileInfo, AppUser user, SystemFile ParentElement, decimal fileSize)
        {
            //Создаем путь.
            string path = String.Empty;
            //В случае, если корневой каталог, путь в корень папки юзера
            if (ParentElement == null)
            {
                path = user.PathToMainFolder + "\\";
            }
            //В случае, если имеется родительский элемент
            else
            {
                //Увелиениче размера папки, в момент добаления файла
                SystemFile f = ParentElement;
                while (f != null)
                {
                    f.Size += fileSize;
                    f = f.ParentFolder;
                }

                path = ParentElement.Path + "\\";
            }

            //Создаем и заполняем обьект файла
            SystemFile sysFile = new SystemFile();
            sysFile.Name = fileInfo.Name;
            sysFile.Uploaded = DateTime.Now;
            sysFile.OwnerId = user.Id;
            sysFile.Path = path;
            sysFile.Size = fileSize;
            sysFile.Sharing_Atribute = (int)ShareType.OwnerOnly; //первоначально файл не расшаренный, а только для юзера
            if (fileInfo.Extension != "")
                sysFile.Format = fileInfo.Extension;
            else
                sysFile.Format = "." + fileInfo.Name;
            sysFile.IsFile = true;

            if (ParentElement != null)
            {
                sysFile.ParentId = ParentElement.Id;
            }

            //Сохраняем файл в БД первоначально, дабы ему дался айдишник. 
            dbContext.SystemFiles.Add(sysFile);
            await dbContext.SaveChangesAsync();

            //Меняем путь, теперь добавлем Id. И пересохраняем. 
            sysFile.Path = sysFile.Path + sysFile.Id.ToString() + sysFile.Name;
            await dbContext.SaveChangesAsync();

            //Возвращаем путь к нему, для заливки в файловую систему
            return sysFile.Path;
        }


        public async Task<string> CreateFolder(string folderName, AppUser user, SystemFile ParentElement)
        {
            //Создаем путь.

            string path = String.Empty;
            //В случае, если корневой каталог, путь в корень папки юзера
            if (ParentElement == null)
            {
                path = user.PathToMainFolder + "\\";
            }
            //В случае, если имеется родительский элемент
            else
            {
                path = ParentElement.Path + "\\";
            }

            //Создаем и заполняем обьект папки
            SystemFile sysFolder = new SystemFile();
            sysFolder.Name = folderName;
            sysFolder.Uploaded = DateTime.Now;
            sysFolder.OwnerId = user.Id;
            sysFolder.Path = path;
            sysFolder.Size = 0;
            sysFolder.Sharing_Atribute = (int)ShareType.OwnerOnly; //первоначально папка не расшаренный, а только для юзера
            sysFolder.Format = "dir";
            sysFolder.IsFile = false;

            if (ParentElement != null)
            {
                sysFolder.ParentId = ParentElement.Id;
            }

            //Сохраняем файл в БД первоначально, дабы ему дался айдишник. 
            dbContext.SystemFiles.Add(sysFolder);
            await dbContext.SaveChangesAsync();

            //Меняем путь, теперь добавлем Id. И пересохраняем. 
            sysFolder.Path = sysFolder.Path + sysFolder.Id.ToString() + sysFolder.Name;
            await dbContext.SaveChangesAsync();

            //Возвращаем путь к папке, для заливки в файловую систему
            return sysFolder.Path;
        }


        //метод для поиска файла или папки по id. 
        public SystemFile GetFileById(int id)
        {
            SystemFile result = (from file in dbContext.SystemFiles
                                 where file.Id == id
                                 select file).FirstOrDefault();
            return result;
        }
        public List<SystemFile> GetUserSharedFiles(AppUser user)
        {
            var result = from file in dbContext.SystemFiles
                         where user.Id == file.OwnerId && file.Sharing_Id != null
                         select file;
            return result.ToList();
        }


        //Метод удаления файла
        public async Task<bool> DeleteSingleFile(SystemFile file)
        {
            if (file == null)
            {
                return false;
            }
            if (file.IsFile)
            {
                if (System.IO.File.Exists(file.Path))
                {
                    try
                    {
                        System.IO.File.Delete(file.Path);
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
            else if (!file.IsFile)
            {
                try
                {
                    System.IO.Directory.Delete(file.Path);
                }
                catch
                {
                    return false;
                }
            }
            SystemFile f = file.ParentFolder;
            while (f != null)
            {
                f.Size -= file.Size;
                f = f.ParentFolder;
            }
            try
            {
                dbContext.SystemFiles.Remove(file);
                await dbContext.SaveChangesAsync();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteSystemFile(int id)
        {
            SystemFile file = GetFileById(id);
            if (file.IsFile)
            {
                return await DeleteSingleFile(file);
            }
            var list_of_children = from li in dbContext.SystemFiles
                                   where li.ParentId == file.Id
                                   select li;

            foreach (var item in list_of_children)
            {
                await DeleteSystemFile(item.Id);
            }
            return await DeleteSingleFile(file);
        }

        public IQueryable<SystemFile> GetFolderContent(int? folderId)
        {
            if (OrderValue == (int)OrderType.ByDate)
            {
                return dbContext.SystemFiles.Where(x => x.ParentId == folderId).OrderBy(m => m.Uploaded);
            }
            else if (OrderValue == (int)OrderType.ByName)
            {
                return dbContext.SystemFiles.Where(x => x.ParentId == folderId).OrderBy(m => m.Name);
            }

            else if (OrderValue == (int)OrderType.ByShareAtribute)
            {
                return dbContext.SystemFiles.Where(x => x.ParentId == folderId).OrderBy(m => m.Sharing_Atribute);
            }

            else if (OrderValue == (int)OrderType.ByFormat)
            {
                return dbContext.SystemFiles.Where(x => x.ParentId == folderId).OrderBy(m => m.Format);
            }
            else
            {
                return dbContext.SystemFiles.Where(x => x.ParentId == folderId).OrderBy(m => m.Size);
            }
        }
        public IQueryable<SystemFile> GetFolderContentWithUser(int? folderId, AppUser user)
        {
            return GetFolderContent(folderId).Where(x => x.OwnerId == user.Id);
        }
        public SystemFile GetFile(int? Id)
        {
            return dbContext.SystemFiles.Where(x => x.Id == Id).FirstOrDefault();
        }


        //Метод для генерирования уникальной строки для шаринга
        public string GenerateRandomString()
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < 5; i++)
            {
                //Генерируем число являющееся латинским символом в юникоде
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                //Конструируем строку со случайно сгенерированными символами
                builder.Append(ch);
            }
            return builder.ToString();
        }
        //Метод для шаринга отдельного файла
        public async Task<bool> ChangeShareStateSingleFile(int shareState, SystemFile file)
        {
            try
            {
                file.Sharing_Atribute = shareState;
                await dbContext.SaveChangesAsync();
            }
            catch
            {
                return false;
            }
            return true;
        }
        //Метод для шаринга обьекта системного файла (файла/папки)
        public async Task<bool> ChangeShareStateSystemFile(int shareState, SystemFile sysFile)
        {
            //Если у нас просто файл, меняем его шаринг атрибут
            if (sysFile.IsFile)
            {
                return await ChangeShareStateSingleFile(shareState, sysFile);
            }

            //если папка, находим все дочерние элементы
            var list_of_files = from s in dbContext.SystemFiles
                                where s.ParentId == sysFile.Id
                                select s;
            //меняем атрибут у всех детей-файлов
            foreach (var item in list_of_files)
            {
                await ChangeShareStateSystemFile(shareState, item);
            }
            //меняем атрибут у самой папки
            return await ChangeShareStateSingleFile(shareState, sysFile);
        }


        //метод для шаринга/аншаринга файла либо папки
        public async Task<string> Share(int shareState, int sysFileId)
        {
            SystemFile sysFile = GetFileById(sysFileId);
            if (sysFile == null)
            {
                return null;
            }
            await ChangeShareStateSystemFile(shareState, sysFile);

            if (shareState == (int)ShareType.ShareReadOnly)
            {   //генерируем уникальную ссылку для файла/папки
                sysFile.Sharing_Id = GenerateRandomString() + sysFile.Id.ToString();
            }
            else
            {
                sysFile.Sharing_Id = null;
            }
            await dbContext.SaveChangesAsync();
            return sysFile.Sharing_Id;
        }

        public int GetFileIdByShareId(string shareID)
        {
            return dbContext.SystemFiles
                .Where(x => x.Sharing_Id == shareID)
                .Select(x => x.Id)
                .FirstOrDefault();
        }

        public SystemFile GetFileByShareId(string shareId)
        {
            return dbContext.SystemFiles.Where(x => x.Sharing_Id == shareId).FirstOrDefault();
        }


        //Получение расшареного файла с проверкой доступа к нему
        //Если доступ разрешен - вернет файл, если запрещен - null
        public SystemFile AccessSharedFile(string link, int? id)
        {
            if (link == null)
                return null;
            SystemFile file;
            if (id != null)
            {
                file = GetFileById(id.Value);
                SystemFile f = file;
                while (f != null)
                {
                    if (f.Sharing_Id != null && link == f.Sharing_Id)
                    {
                        return file;
                    }
                    f = f.ParentFolder;
                }
                return null;
            }
            else
            {
                file = GetFileByShareId(link);
                return file;
            }
        }

        public string ArchiveTheFolder(SystemFile folder)
        {
            string zipPath = folder.Path + ".zip";
            ZipFile.CreateFromDirectory(folder.Path, zipPath);

            return zipPath;
        }

        //Ищем файл
        public List<SystemFile> SearchFile(string searchEx)
        {
            var result = from f in dbContext.SystemFiles
                         where f.Name.Contains(searchEx) == true
                         select f;
            return result.ToList();
        }
        //Ищем файл у юзера
        public List<SystemFile> SearchFileForUser(string _searchEx, AppUser _user)
        {
            return SearchFile(_searchEx).Where(f => f.OwnerId == _user.Id).ToList();
        }
        //Ищем файл внутри определенной папки (для использования в шарринг контроллере) 
        public List<SystemFile> SearchFileInParentFolder(string _searchEx, int parentFolderId)
        {
            return SearchFile(_searchEx).Where(f => f.ParentId == parentFolderId).ToList();
        }
    }

    public enum OrderType
    {
        ByDate = 1,
        ByName = 2,
        ByShareAtribute = 3,
        ByFormat = 4,
        BySize = 5
    }
}
