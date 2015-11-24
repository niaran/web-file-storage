using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebStorage.Domain.Abstract;
using Microsoft.Owin;
using WebStorage.Domain.Concrete;
using System.IO;

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
                ParentElement.Size = ParentElement.Size + fileSize;

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
            sysFile.Format = fileInfo.Extension;
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
            if (file.ParentFolder != null)
            {
                file.ParentFolder.Size = file.ParentFolder.Size - file.Size;
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
            return dbContext.SystemFiles.Where(x => x.ParentId == folderId);
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
                return String.Empty;
            }
            await ChangeShareStateSystemFile(shareState, sysFile);
            //генерируем уникальную ссылку для файла/папки
            sysFile.Sharing_Id = GenerateRandomString() + sysFile.Id.ToString();
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
    }
}
