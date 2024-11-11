using System.ComponentModel.DataAnnotations;

namespace BoardGamesStore.Models
{
    public class Category
    {
        public int CategoryID { get; set; }

        [Display(Name = "Category name")]
        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters.")]
        public string CategoryName { get; set; } = null!;

        public ICollection<BoardGame>? BoardGames { get; set; }
    }
}
