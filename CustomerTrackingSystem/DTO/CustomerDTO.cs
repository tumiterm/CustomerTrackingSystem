using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace CustomerTrackingSystem.DTO
{
    public class CustomerDTO
    {
        public Guid CustomerId { get; set; }
        public string? VATNumber { get; set; }
        public string CustomerName { get; set; }
        public string StreetComplex { get; set; }
        public string City { get; set; }
        public string Surburb { get; set; }
        public string PostalCode { get; set; }
        public string? Telephone { get; set; }
        public string? ContactPersonName { get; set; }
        public string? ContactPersonEmail { get; set; }

    }
}
