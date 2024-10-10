namespace BoardGamesStore.Models
{
    public class OrderDetail
    {
        public int OrderID { get; set; }

        public Order Order { get; set; }


        public int BoardGameID { get; set; }

        public BoardGame BoardGame { get; set; }


        public int Quantity { get; set; }

        public decimal Price { get; set; }
    }
}
