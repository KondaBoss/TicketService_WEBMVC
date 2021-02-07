using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketService.Data.Models
{
    public class ShoppingTicket
    {
        public int ID { get; set; }

        public int EventID { get; set; }

        public string EventName { get; set; }

        public int NumberOfTicketsRemaining { get; set; }

        public int NumberOfTicketsToBuy { get; set; }

        public ETicketType TypeOfTicket { get; set; }

        public double SingleTicketPrice { get; set; }
    }
}
