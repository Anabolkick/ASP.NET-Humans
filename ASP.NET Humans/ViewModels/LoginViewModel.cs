using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASP.NET_Humans.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Login or Email")]
        public string Email_Login { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember?")]
        public bool IsRemember { get; set; }
        public string ReturnUrl { get; set; }

    }
}
