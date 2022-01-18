using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ASP.NET_Humans.ViewModels
{
    public class EditUserViewModel
    {
        public string Login { get; set; }
        public string Email { get; set; }
        public string Id { get; set; }

        [Display(Name = "Email Confirmed")]
        public bool EmailConfirmed { get; set; }
        public List<string> Roles { get; set; }
        
    }
}
