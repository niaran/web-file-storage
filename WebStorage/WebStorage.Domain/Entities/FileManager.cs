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
        private AppDbContext dbContext;
        public FileManager()
        {
            dbContext = new AppDbContext();
        }

        public FileManager(AppDbContext db)
        {
            dbContext = db;
        }

        /*public static FileManager Create(IOwinContext context)
        {
            AppDbContext db = context.Get<AppDbContext>(typeof(AppDbContext).ToString());
            FileManager manager = new FileManager(db);
            return manager;
        }*/
    }
}
