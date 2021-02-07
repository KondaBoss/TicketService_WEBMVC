using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketService.Data.Services
{
    public class DataContext
    {
        public DataContext()
        {
            Administrator = new AdministratorData();
            Comment = new CommentData();
            Customer = new CustomerData();
            CustomerType = new CustomerTypeData();
            Event = new EventData();
            Location = new LocationData();
            Salesman = new SalesmanData();
            Ticket = new TicketData();
            Venue = new VenueData();
        }
        
        public AdministratorData Administrator { get; private set; }
        public CommentData Comment { get; private set; }
        public CustomerData Customer { get; private set; }
        public CustomerTypeData CustomerType { get; private set; }
        public EventData Event { get; private set; }
        public LocationData Location { get; private set; }
        public SalesmanData Salesman { get; private set; }
        public TicketData Ticket { get; private set; }
        public VenueData Venue { get; private set; }
    }
}
