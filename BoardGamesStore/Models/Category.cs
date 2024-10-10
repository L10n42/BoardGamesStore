namespace BoardGamesStore.Models
{
    public class Category
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } = null!;

        public ICollection<BoardGame>? BoardGames { get; set; }
    }
}
