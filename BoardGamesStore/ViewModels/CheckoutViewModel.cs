using BoardGamesStore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BoardGamesStore.ViewModels
{
    public class CheckoutViewModel
    {

        [Display(Name = "FirstName")]
        [Required(ErrorMessage = "FirstNameIsRequired")]
        public string FirstName { get; set; }

        [Display(Name = "LastName")]
        [Required(ErrorMessage = "LastNameIsRequired")]
        public string LastName { get; set; }

        [Display(Name = "PhoneNumber")]
        [Required(ErrorMessage = "PhoneNumberIsRequired")]
        [Phone]
        public string PhoneNumber { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "EmailIsRequired")]
        [EmailAddress]
        public string Email { get; set; }


        [Display(Name = "DeliveryOption")]
        [Required(ErrorMessage = "DeliveryOptionRequired")]
        public string DeliveryOption { get; set; }

        public SelectList AvailableDeliveryOptions { get; set; } = new SelectList(Enumerable.Empty<SelectListItem>());


        [Display(Name = "PaymentMethod")]
        [Required(ErrorMessage = "PaymentMethodRequired")]
        public string PaymentMethod { get; set; }

        public SelectList AvailablePaymentMethods { get; set; } = new SelectList(Enumerable.Empty<SelectListItem>());

        [Display(Name = "CardNumber")]
        [RegularExpression(@"^\d{16}$", ErrorMessage = "CardNumberFormatError")]
        public string? CardNumber { get; set; }

        [Display(Name = "ExpiryDate")]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/?([0-9]{2})$", ErrorMessage = "ExpiryDateFormatError")]
        public string? ExpiryDate { get; set; }

        [Display(Name = "CVV")]
        [RegularExpression(@"^\d{3,4}$", ErrorMessage = "CVVFormatError")]
        public string? CVV { get; set; }


        public List<CartBoardGame> Items { get; set; } = new List<CartBoardGame>();

        [Display(Name = "TotalAmount")]
        public double TotalAmount { get; set; }

    }
}
