using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoardGamesStore.Models
{
    public class BoardGame
    {
        public int BoardGameID { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string Description { get; set; } = null!;

        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
        public double Price { get; set; }

        [Display(Name = "Stock quantity")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be a positive value.")]
        public int StockQuantity { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Number of players")]
        [Required(ErrorMessage = "Number of players is required.")]
        [RegularExpression(@"^\d+(-\d+)?\+?$", ErrorMessage = "Enter a valid format for number of players (e.g., '2', '2-3', '2+').")]
        public string NumberOfPlayers { get; set; } = null!;

        [Display(Name = "Game time (minutes)")]
        [Required(ErrorMessage = "Game time is required.")]
        [RegularExpression(@"^\d+(-\d+)?\+?$", ErrorMessage = "Enter a valid format for game time in minutes (e.g., '30', '30-60', '30+').")]
        public string GameTimeMinutes { get; set; } = null!;

        [Display(Name = "Suggested age")]
        [Required(ErrorMessage = "Suggested age is required.")]
        [RegularExpression(@"^\d+\+$", ErrorMessage = "Enter a valid age suggestion (e.g., '8+').")]
        public string SuggestedAge { get; set; } = null!;


        [Display(Name = "Category")]
        [Range(1, int.MaxValue, ErrorMessage = "Choose category")]
        public int? CategoryID { get; set; }

        [ForeignKey("CategoryID")]
        public Category? Category { get; set; }


        public ICollection<OrderDetail>? OrderDetails { get; set; }

        public ICollection<CartBoardGame>? CartBoardGames { get; set; }

        public ICollection<BoardGameImage>? BoardGameImages { get; set; }
    }
}
