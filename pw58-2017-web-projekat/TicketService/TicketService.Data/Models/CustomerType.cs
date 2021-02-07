using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TicketService.Data.Models
{
    public class CustomerType
    {
        public int ID { get; set; }

        [Required]
        [DisplayName("Customer Type")]
        [StringLength(6)]
        public string Name { get; set; }

        [DisplayName("Required number of points")]
        public int NumberOfPoints { get; set; }

        public double Discount { get; set; }

        public bool IsDeleted { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb
                .Append(ID.ToString())
                .Append("|")
                .Append(Name ?? string.Empty)
                .Append("|")
                .Append(NumberOfPoints.ToString())
                .Append("|")
                .Append(Discount.ToString())
                .Append("|")
                .Append(IsDeleted.ToString());

            return sb.ToString();
        }
    }
}
