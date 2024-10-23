using BoardGamesStore.Models;

namespace BoardGamesStore.ViewModels
{
    public class CartViewModel
    {
        public IEnumerable<CartBoardGame> Items { get; set; }

        public double TotalPrice { get; set; }

        public double TotalItems { get; set; }
    }
}
