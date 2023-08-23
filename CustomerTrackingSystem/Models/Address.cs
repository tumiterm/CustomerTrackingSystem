using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerTrackingSystem.Models
{
    public class Address
    {
        [Key]
        public Guid AddressId { get; set; }

        [ForeignKey(nameof(Customer))] 
        public Guid CustomerId { get; set; }

        [Display (Name = "Street/Complex No")]
        [Required]
        public string StreetComplex { get; set; }

        [Required]
        public string City { get; set; }    
        public string Surburb { get; set; }

        [Display(Name = "Postal Code")]
        [Required]
        public string PostalCode { get; set; }
    }
}
