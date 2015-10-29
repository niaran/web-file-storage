using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using WebStorage.Domain.Entities;

namespace WebStorage.Domain.Concrete
{
    class IdentityContext : DbContext 
    {
        public IdentityContext() :base("DefaultConnection")
        { }

        public DbSet<IdentityUser> Users { get; set; }
        //public DbSet<UserLogin> UserLogins { get; set; }
    }
}
