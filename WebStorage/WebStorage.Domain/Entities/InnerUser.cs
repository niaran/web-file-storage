using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebStorage.Domain.Entities
{
    public class InnerUser
    {
        [Key]
        [ForeignKey("AppUser")]
        public String Id { get; set; }
        
        public AppUser AppUser { get; set; }

        public virtual ICollection<SystemFile> Files { get; set; }

        public InnerUser()
        {
            Files = new List<SystemFile>();
        }
    }
}
