using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using TicketService.Data.Interfaces;
using TicketService.Data.Models;

namespace TicketService.Data.Services
{
    public class EventData : IEventData
    {
        public IEnumerable<Event> GetAllForSalesman(int salesmanID)
        {
            return GetEvents().Where(x => x.SalesmanID == salesmanID).ToList();
        }

        public IEnumerable<Event> GetAll()
        {
            return GetEvents();
        }

        public static void SaveEvents(List<Event> events)
        {
            string path = HostingEnvironment.MapPath("~/App_Data/Events.txt");
            string temp = string.Empty;

            foreach (Event e in events)
            {
                temp = temp += e.ToString() + "\n";
            }

            System.IO.File.WriteAllText(path, temp);
        }

        public static void SaveEvent(Event e)
        {
            string path = HostingEnvironment.MapPath("~/App_Data/Events.txt");
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(e.ToString());
            }
        }


        public static List<Event> GetEvents()
        {
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "App_Data/Events.txt"))
                return new List<Event>();

            List<Event> list = new List<Event>();


            using (StreamReader reader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "App_Data/Events.txt"))
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

        public static Event FromString(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            var fields = str.Split('|');


            //DateTime date7 = new DateTime(1999, 1, 19, 20, 50, 0);
            //var date7str = date7.ToShortDateString();
            //date7str = date7.ToShortTimeString();
            //date7str = date7.ToLongDateString();
            //date7str = date7.ToLongTimeString();
            //date7str = date7.ToString();

            ////var dejt1 = DateTime.ParseExact(fields[5], "0:yyyy-MM-dd HH:mm tt",
            ////                           System.Globalization.CultureInfo.InvariantCulture);

            DateTime someDate = new DateTime();
            DateTime.TryParse(fields[5], out someDate);

            var retEvent = new Event()
            {
                ID = int.Parse(fields[0]),
                SalesmanID = int.Parse(fields[1]),
                VenueID = int.Parse(fields[2]),
                Name = fields[3],
                Poster = fields[4],
                EventDate = someDate,
                EventType = (EEventType)Enum.Parse(typeof(EEventType), fields[6]),
                TicketPrice = double.Parse(fields[7]),
                Capacity = int.Parse(fields[8]),
                Status = bool.Parse(fields[9]),
                IsDeleted = bool.Parse(fields[10]),
            };



            return retEvent;
        }

        public static string ListToString(List<Event> list)
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
