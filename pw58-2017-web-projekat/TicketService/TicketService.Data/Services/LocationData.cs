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
    public class LocationData : ILocationData
    {
        public IEnumerable<Location> GetAll()
        {
            return GetLocations();
        }
        public Location GetLocation(int venueID)
        {
            return GetLocations().SingleOrDefault(x => x.VenueID == venueID);
        }

        public static List<Location> GetLocations()
        {
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "App_Data/Locations.txt"))
                return new List<Location>();

            List<Location> list = new List<Location>();


            using (StreamReader reader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "App_Data/Locations.txt"))
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

        public static Location FromString(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            var fields = str.Split('|');

            var location = new Location()
            {
                ID = int.Parse(fields[0]),
                VenueID = int.Parse(fields[1]),
                Longitude = int.Parse(fields[2]),
                Latitude = int.Parse(fields[3]),
                IsDeleted = bool.Parse(fields[4])
            };

            return location;
        }

        public static string ListToString(List<Location> list)
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
