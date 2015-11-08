using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebStorage.UI.Models
{
    public class ExternalLoginViewModel
    {
        [Required]
        [DisplayName ("Email")]
        public String Email { get; set; }

        [Required]
        [DisplayName("Name")]
        public String Name { get; set; }
    }
}