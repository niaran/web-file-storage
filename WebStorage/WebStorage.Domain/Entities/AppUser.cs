using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using System.IO;
using System.Security.Claims;
using Microsoft.AspNet.Identity;

namespace WebStorage.Domain.Entities
{
    public class AppUser : IdentityUser
    {
        // путь к корню облачного хранилища
        private String _pathToBaseDirectory;
        //public InnerUser InnerUser { get; set; }

        public AppUser() : base()
        {
            _pathToBaseDirectory = @"C:\Users\Public";
            Files = new List<SystemFile>();
        }

        public String PathToMainFolder { get; set; }

        public virtual ICollection<SystemFile> Files { get; set; }

        // Создаем корневую папку пользователя, 
        // после создания пользователя
        public void CreateMainFolder()
        {
            // путь к папке пользователя в виде строки
            String pathToAppUserDirectory = Path.Combine(_pathToBaseDirectory,
                this.UserName + "-" + this.Id);
            // если такой папки не существует создаем ее
            if (!Directory.Exists(pathToAppUserDirectory) && !String.IsNullOrEmpty(pathToAppUserDirectory))
            {
                Directory.CreateDirectory(pathToAppUserDirectory);
                PathToMainFolder = pathToAppUserDirectory;
            }                        
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<AppUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}
