using System.ComponentModel.DataAnnotations.Schema;

namespace BoardGamesStore.Models
{
    public class Order
    {
        public int OrderID { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }

        public string OrderStatus { get; set; } = null!;


        public ICollection<OrderDetail> OrderDetails { get; set; }


        public string UserID { get; set; }

        [ForeignKey("UserID")]
        public ApplicationUser User { get; set; }
    }
}
