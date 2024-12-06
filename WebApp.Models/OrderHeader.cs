using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Models
{
    public class OrderHeader
    {
        public int Id { get; set; }

        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicatoinUserId")]
        [ValidateNever]
        public ApplicationUsers ApplicationUsers { get; set; }

        public DateTime OrderDate { get; set; }
        public DateTime ShippingDate { get; set; }
        public double OrderTotal { get; set; }

        public string? OrderStatus { get; set; }
        public string? PaymenetStatus { get; set; }
        public string? TrackingNumber { get; set; }
        public string? Carrier { get; set; }

        public DateTime PaymentDate { get; set; }
        public DateOnly PaymentDueDate { get; set; }

        public string? PayementIntentId { get; set; }


        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string StreedAddress { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string PostalCode { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
