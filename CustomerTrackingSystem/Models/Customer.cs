using System.ComponentModel.DataAnnotations;

namespace CustomerTrackingSystem.Models
{
    public class Customer
    {
        [Key]
        public Guid CustomerId { get; set; }

        [Required]
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        [Required]
        public Address Address { get; set; }
        public Contact? ContactPerson { get; set; }
        public string? VATNumber { get; set; }
    }
}
