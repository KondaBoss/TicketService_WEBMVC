using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace TicketService.Data.Models
{
    public class Location
    {
        public int ID { get; set; }

        public int VenueID { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public bool IsDeleted { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb
                .Append(ID.ToString())
                .Append("|")
                .Append(VenueID.ToString())
                .Append("|")
                .Append(Longitude.ToString())
                .Append("|")
                .Append(Latitude.ToString())
                .Append("|")
                .Append(IsDeleted.ToString());

            return sb.ToString();
        }
    }
}
