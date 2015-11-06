﻿using System;
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

    }
}
