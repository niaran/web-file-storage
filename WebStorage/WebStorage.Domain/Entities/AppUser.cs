using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace WebStorage.Domain.Entities
{
    public class AppUser : IdentityUser
    {
        public InnerUser InnerUser { get; set; }
    }
}
