using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using WebStorage.Domain.Entities;

namespace WebStorage.Domain.Concrete
{
    class IdentityUserContext : DbContext 
    {
        public IdentityUserContext() :base("DbConnection")
        { }

        public DbSet<IdentityUser> Users { get; set; }
    }
}
