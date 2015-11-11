using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebStorage.Domain.Entities
{
    public class SystemFile
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime Uploaded { get; set; }
                
        [Required]        
        public String OwnerId { get; set; }
        //public String OwnerId { get; set; }

        [ForeignKey("OwnerId")]
        //public virtual InnerUser Owner { get; set; }
        public virtual AppUser Owner { get; set; }

        [Required]
        public string Path { get; set; }

        [Required]
        public decimal Size { get; set; }

        [Required]
        public string Format { get; set; }


        public int Sharing_Atribute { get; set; }

        public string Sharing_Id { get; set; }

        public int? ParentId { get; set; }

        [ForeignKey("ParentId")]
        public virtual SystemFile ParentFolder { get; set; }

        [Required]
        public bool IsFile { get; set; }

        public string SizeAsMemory()
        {
            const int scale = 1024;
            string[] orders = new string[] { "GB", "MB", "KB", "Bytes" };
            long max = (long)Math.Pow(scale, orders.Length - 1);

            foreach (string order in orders)
            {
                if (Size > max)
                    return string.Format("{0:##.##} {1}", decimal.Divide(Size, max), order);

                max /= scale;
            }
            return "0 Bytes";
        }
    }

    public enum ShareType
    {
        OwnerOnly = 1,
        ShareReadOnly = 2,
        ShareEdit = 3
    }
}
