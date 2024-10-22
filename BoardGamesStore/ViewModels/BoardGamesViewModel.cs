using BoardGamesStore.Models;

namespace BoardGamesStore.ViewModels
{
    public class BoardGamesViewModel
    {
        public IEnumerable<BoardGame> BoardGames { get; set; }


        public string SearchQuery { get; set; } = string.Empty;
        public int? CategoryID { get; set; }
        public double? MinPrice { get; set; }
        public double? MaxPrice { get; set; }


        public string SortBy { get; set; } = "Date";
        public string SortOrder { get; set; } = "asc";


        public IEnumerable<Category> Categories { get; set; }
    }
}
