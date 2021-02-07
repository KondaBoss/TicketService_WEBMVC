using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketService.Data.Interfaces
{
    public interface IUser
    {
        int ID { get; set; }
        DateTime BirthDate { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        bool Gender { get; set; }
        string UserName { get; set; }
        string Password { get; set; }
    }
}
