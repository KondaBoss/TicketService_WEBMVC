using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TicketService.Data.Models
{
    public class Venue
    {
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Address { get; set; }

        [Required]
        [StringLength(100)]
        public string City { get; set; }

        [Required]
        [StringLength(10)]
        [DataType(DataType.PostalCode)]
        public string ZIPCode { get; set; }

        public bool IsDeleted { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb
                .Append(ID.ToString())
                .Append("|")
                .Append(Name ?? string.Empty)
                .Append("|")
                .Append(Address ?? string.Empty)
                .Append("|")
                .Append(City ?? string.Empty)
                .Append("|")
                .Append(ZIPCode ?? string.Empty)
                .Append("|")
                .Append(IsDeleted.ToString());

            return sb.ToString();
        }
    }
}
