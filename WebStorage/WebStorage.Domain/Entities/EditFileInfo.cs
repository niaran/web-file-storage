using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebStorage.Domain.Entities
{
    public class EditFileInfo
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public DateTime Edited { get; set; }

        [Required]
        public int FileId { get; set; }

        [Required]
        [ForeignKey("FileId")]
        public virtual SystemFile EditedFile { get; set; }
    }
}
