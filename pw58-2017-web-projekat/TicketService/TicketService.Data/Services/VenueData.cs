using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketService.Data.Interfaces;
using TicketService.Data.Models;

namespace TicketService.Data.Services
{
    public class VenueData : IVenueData
    {
        public Venue GetVenue(int eventID)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Venue> GetAll()
        {
            return GetVenues();
        }

        public static List<Venue> GetVenues()
        {
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "App_Data/Venues.txt"))
                return new List<Venue>();

            List<Venue> list = new List<Venue>();


            using (StreamReader reader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "App_Data/Venues.txt"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (FromString(line) != null)
                        list.Add(FromString(line));
                }
            }
            return list;
        }

        public static Venue FromString(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            var fields = str.Split('|');

            var venue = new Venue()
            {
                ID = int.Parse(fields[0]),
                Name = fields[1],
                Address = fields[2],
                City = fields[3],
                ZIPCode = fields[4],
                IsDeleted = bool.Parse(fields[5])
            };

            return venue;
        }

        public static string ListToString(List<Venue> list)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                sb.Append(list[i].ToString());
            }
            return sb.ToString();
        }
    }
}
