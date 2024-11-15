using System.ComponentModel.DataAnnotations;

namespace BoardGamesStore.ViewModels
{
    public class LoginViewModel
    {

        [Display(Name = "Email")]
        [Required(ErrorMessage = "EmailIsRequired")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "PasswordIsRequired")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "RememberMe")]
        public bool RememberMe { get; set; }

    }
}
