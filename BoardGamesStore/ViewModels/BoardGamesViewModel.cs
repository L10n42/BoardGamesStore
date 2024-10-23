using BoardGamesStore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BoardGamesStore.ViewModels
{
    public class BoardGamesViewModel
    {
        public IEnumerable<BoardGame> BoardGames { get; set; } = Enumerable.Empty<BoardGame>();


        public string SearchQuery { get; set; } = string.Empty;
        public int? CategoryID { get; set; }
        public double? MinPrice { get; set; }
        public double? MaxPrice { get; set; }


        public string SortBy { get; set; } = "Date";
        public string SortOrder { get; set; } = "asc";


        public SelectList Categories { get; set; }
        public SelectList SortOptions { get; set; }
        public SelectList SortOrderOptions { get; set; }

    }
}
