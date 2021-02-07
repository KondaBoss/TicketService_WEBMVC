using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketService.Data.Models;

namespace TicketService.Data.Interfaces
{
    public interface ITicketData
    {
        IEnumerable<Ticket> GetAll();
        IEnumerable<Ticket> GetAllForCustomer(int customerID);
        IEnumerable<Ticket> GetAllForSalesman(int salesmanID);
    }
}
