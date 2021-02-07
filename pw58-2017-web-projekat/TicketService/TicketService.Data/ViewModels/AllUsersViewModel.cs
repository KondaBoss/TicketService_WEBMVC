using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketService.Data.Models;

namespace TicketService.Data.ViewModels
{
    public class AllUsersViewModel
    {
        public IEnumerable<Administrator> Administrators { get; set; }
        public IEnumerable<Salesman> Salesmen { get; set; }
        public IEnumerable<Customer> Customers { get; set; }
    }
}
