using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketService.Data.Models;

namespace TicketService.Data.Interfaces
{
    public interface ICustomerData
    {
        IEnumerable<Customer> GetAll();
        IEnumerable<Customer> GetAllForSalesman(int eventID);
    }
}
