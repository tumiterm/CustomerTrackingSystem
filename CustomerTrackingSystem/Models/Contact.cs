using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerTrackingSystem.Models
{
    public class Contact
    {
        [Key]
        public Guid ContactId { get; set; }

        [ForeignKey(nameof(Customer))]
        public Guid CustomerId { get; set; }
        public string? Telephone { get; set; }

        [Display(Name = "Contact Person Name")]
        public string? ContactPersonName { get; set; }

        [Display(Name = "Contact Person Email")]
        [DataType(DataType.EmailAddress)]
        [StringLength(100, ErrorMessage = "Email address is too long")]
        public string? ContactPersonEmail { get; set; }

    }
}
