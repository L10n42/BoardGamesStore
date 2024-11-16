using System.ComponentModel.DataAnnotations;

namespace BoardGamesStore.ViewModels
{
    public class RegisterViewModel
    {

        [Display(Name = "Email")]
        [Required(ErrorMessage = "EmailIsRequired")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "NameIsRequired")]
        public string Name { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "PasswordIsRequired")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "ConfirmPassword")]
        [Required(ErrorMessage = "ConfirmPasswordIsRequired")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "PasswordsDoNotMatch")]
        public string ConfirmPassword { get; set; }

    }
}
