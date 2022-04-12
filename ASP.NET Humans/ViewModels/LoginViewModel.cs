using System.ComponentModel.DataAnnotations;


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
