using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BoardGamesStore.ViewModels
{
    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "EmailIsRequired")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "NameIsRequired")]
        public string Name { get; set; }

        [Display(Name = "Role")]
        [Required(ErrorMessage = "RoleIsRequired")]
        public string SelectedRole { get; set; }

        public SelectList Roles { get; set; } = new SelectList(Enumerable.Empty<SelectListItem>());
    }
}
