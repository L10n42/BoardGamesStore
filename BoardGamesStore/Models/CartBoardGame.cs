using BoardGamesStore.Areas.Identity.Data;

namespace BoardGamesStore.Models
{
    public class CartBoardGame
    {
        public string UserID { get; set; }

        public ApplicationUser User { get; set; }


        public int BoardGameID { get; set; }

        public BoardGame BoardGame { get; set; }


        public int Quantity { get; set; }
    }
}
