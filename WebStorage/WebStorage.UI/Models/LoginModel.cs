using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WebStorage.UI.Models
{
    public class LoginModel
    {
        [Required]
        public String Name { get; set; }
        [Required]
        public String Password { get; set; }
        [Required]
        public Boolean remember { get; set; }
    }
}