using BoardGamesStore.Models;

namespace BoardGamesStore.ViewModels
{
    public class HomeViewModel
    {
        public List<BoardGame> RecentArrivals { get; set; } = new List<BoardGame>();
        public List<BoardGame> BestSellingGames { get; set; } = new List<BoardGame>();
    }
}
