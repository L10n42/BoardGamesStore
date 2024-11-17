using System.ComponentModel.DataAnnotations.Schema;

namespace BoardGamesStore.Models
{
    public class Order
    {
        public int OrderID { get; set; }

        public DateTime OrderDate { get; set; }

        public double TotalAmount { get; set; }

        public string OrderStatus { get; set; } = null!;

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public string DeliveryOption { get; set; }
        public string PaymentMethod { get; set; }

        public string CardNumber { get; set; }
        public string ExpiryDate { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }


        public string UserID { get; set; }

        [ForeignKey("UserID")]
        public ApplicationUser User { get; set; }
    }
}
