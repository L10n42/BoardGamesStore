using System.ComponentModel.DataAnnotations.Schema;

namespace BoardGamesStore.Models
{
    public class Review
    {
        public int ReviewID { get; set; }

        public int Rating { get; set; }

        public string Comment { get; set; }

        public string? UserID { get; set; }

        public DateTime ReviewDate { get; set; }

        [ForeignKey("UserID")]
        public ApplicationUser? User { get; set; }

        public int? BoardGameID { get; set; }

        [ForeignKey("BoardGameID")]
        public BoardGame? BoardGame { get; set; }
    }
}
