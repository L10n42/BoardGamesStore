using System.ComponentModel.DataAnnotations.Schema;

namespace BoardGamesStore.Models
{
    public class BoardGameImage
    {
        public int BoardGameImageID { get; set; }

        public string ImageUrl { get; set; } = null!;


        public int? BoardGameID { get; set; }

        [ForeignKey("BoardGameID")]
        public BoardGame? BoardGame { get; set; }
    }
}
