using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TicketService.Data.Models
{
    public class Ticket
    {
        public int ID { get; set; }

        [DataType(DataType.Currency)]
        [DisplayName("Ticket price")]
        public double Price { get; set; }

        public int CustomerID { get; set; }

        public int SalesmanID { get; set; }

        public int EventID { get; set; }

        [DisplayName("Ticket status")]
        public bool Status { get; set; }

        [DisplayName("Type of ticket")]
        public ETicketType Type { get; set; }

        public bool IsDeleted { get; set; }
        
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb
                .Append(ID.ToString())
                .Append("|")
                .Append(CustomerID.ToString())
                .Append("|")
                .Append(SalesmanID.ToString())
                .Append("|")
                .Append(EventID.ToString())
                .Append("|")
                .Append(Type.ToString())
                .Append("|")
                .Append(Price.ToString())
                .Append("|")
                .Append(Status.ToString())
                .Append("|")
                .Append(IsDeleted.ToString());

            return sb.ToString();
        }
    }
}
