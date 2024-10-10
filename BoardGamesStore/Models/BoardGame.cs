using System.ComponentModel.DataAnnotations.Schema;

namespace BoardGamesStore.Models
{
    public class BoardGame
    {
        public int BoardGameID { get; set; }

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public double Price { get; set; }

        public int StockQuantity { get; set; }

        public DateTime CreatedAt { get; set; }

        public string NumberOfPlayers { get; set; } = null!;

        public string GameTimeMinutes { get; set; } = null!;

        public string SuggestedAge { get; set; } = null!;


        public int? CategoryID { get; set; }

        [ForeignKey("CategoryID")]
        public Category? Category { get; set; }


        public ICollection<OrderDetail>? OrderDetails { get; set; }

        public ICollection<CartBoardGame>? CartBoardGames { get; set; }

        public ICollection<BoardGameImage>? BoardGameImages { get; set; }
    }
}
