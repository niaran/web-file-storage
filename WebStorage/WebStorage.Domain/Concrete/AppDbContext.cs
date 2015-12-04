using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using WebStorage.Domain.Entities;

namespace WebStorage.Domain.Concrete
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext() : base("IdentityDb")
        { }

        public DbSet<EditFileInfo> EditFileInfoes { get; set; }
        public DbSet<SystemFile> SystemFiles { get; set; }
        
        static AppDbContext()
        {
            Database.SetInitializer<AppDbContext>(new IdentityDbInit());
        }

        public static AppDbContext Create()
        {
            return new AppDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SystemFile>()
                .HasRequired(o => o.Owner)
                .WithMany()
                .WillCascadeOnDelete(false);
            

            base.OnModelCreating(modelBuilder);
        }

        
    }

    // Класс, кот можно использовать для разнообразных настроек 
    public class IdentityDbInit :  DropCreateDatabaseIfModelChanges<AppDbContext> 
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
