using System.ComponentModel.DataAnnotations;

namespace BoardGamesStore.Models
{
    public class Category
    {
        public int CategoryID { get; set; }

        [Display(Name = "CategoryName")]
        [Required(ErrorMessage = "CategoryNameIsRequired")]
        [StringLength(100, ErrorMessage = "CategoryNameLimit")]
        public string CategoryName { get; set; } = null!;

        public ICollection<BoardGame>? BoardGames { get; set; }
    }
}
