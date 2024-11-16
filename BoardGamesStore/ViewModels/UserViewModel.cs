using System.ComponentModel.DataAnnotations;

namespace BoardGamesStore.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Role")]
        public string Role { get; set; }
    }
}
