using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketService.Data.Models
{
    public class ShoppingCart
    {
        public int ID { get; set; }

        public int CustomerID { get; set; }

        public List<ShoppingTicket> Tickets { get; set; }
        
        public double TotalPrice
        {
            get
            {
                double num = 0;
                foreach (var item in Tickets)
                {
                    num += item.NumberOfTicketsToBuy * item.SingleTicketPrice;
                }
                return num;
            }
        }

        public ShoppingCart()
        {
            Tickets = new List<ShoppingTicket>();
        }
    }
}
