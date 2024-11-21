using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoardGamesStore.Models
{
    public class BoardGame
    {
        public int BoardGameID { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "NameIsRequired")]
        [StringLength(100, ErrorMessage = "NameLimit")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        [Required(ErrorMessage = "DescriptionIsRequired")]
        [StringLength(1000, ErrorMessage = "DescriptionLimit")]
        public string Description { get; set; }

        [Display(Name = "Price")]
        [Required(ErrorMessage = "PriceIsRequired")]
        [Range(0, double.MaxValue, ErrorMessage = "PositivePrice")]
        public double Price { get; set; }

        [Display(Name = "StockQuantity")]
        [Required(ErrorMessage = "StockQuantityIsRequired")]
        [Range(0, int.MaxValue, ErrorMessage = "PositiveStockQuantity")]
        public int StockQuantity { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "NumberOfPlayers")]
        [Required(ErrorMessage = "NumberOfPlayersIsRequired")]
        [RegularExpression(@"^\d+(-\d+)?\+?$", ErrorMessage = "NumberOfPlayersInvalidFormat")]
        public string NumberOfPlayers { get; set; }

        [Display(Name = "GameTime")]
        [Required(ErrorMessage = "GameTimeIsRequired")]
        [RegularExpression(@"^\d+(-\d+)?\+?$", ErrorMessage = "GameTimeInvalidFormat")]
        public string GameTimeMinutes { get; set; }

        [Display(Name = "SuggestedAge")]
        [Required(ErrorMessage = "SuggestedAgeIsRequired")]
        [RegularExpression(@"^\d+\+$", ErrorMessage = "SuggestedAgeInvalidFormat")]
        public string SuggestedAge { get; set; }


        [Display(Name = "Category")]
        [Range(1, int.MaxValue, ErrorMessage = "ChooseСategory")]
        public int? CategoryID { get; set; }

        [ForeignKey("CategoryID")]
        public Category? Category { get; set; }

        public ICollection<CartBoardGame>? CartBoardGames { get; set; }
        public ICollection<OrderDetail>? OrderDetails { get; set; }
        public ICollection<BoardGameImage>? BoardGameImages { get; set; }
        public ICollection<Review>? Reviews { get; set; }
    }
}
