using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using WebStorage.Domain.Entities;

namespace WebStorage.Domain.Concrete
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext() : base("IdentityDb")
        { }

        DbSet<InnerUser> InnerUsers { get; set; }
        DbSet<EditFileInfo> EditFileInfo { get; set; }
        DbSet<SystemFile> SystemFiles { get; set; }
        
        static AppDbContext()
        {
            Database.SetInitializer<AppDbContext>(new IdentityDbInit());
        }

        public static AppDbContext Create()
        {
            return new AppDbContext();
        }
    }

    // Класс, кот можно использовать для разнообразных настроек 
    public class IdentityDbInit : DropCreateDatabaseIfModelChanges<AppDbContext> // DropCreateDatabaseAlways
    {
        protected override void Seed(AppDbContext context)
        {
            PerformInitialSetup(context);
            base.Seed(context);
        }

        private void PerformInitialSetup(AppDbContext context)
        {
            //throw new NotImplementedException();
        }
    }
}
