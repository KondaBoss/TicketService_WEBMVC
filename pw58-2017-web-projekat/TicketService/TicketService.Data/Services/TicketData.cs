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
    public class TicketData : ITicketData
    {
        public IEnumerable<Ticket> GetAllForCustomer(int customerID)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Ticket> GetAllForSalesman(int salesmanID)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Ticket> GetAll()
        {
            return GetTickets();
        }

        public static void SaveTickets(List<Ticket> tickets)
        {
            string path = HostingEnvironment.MapPath("~/App_Data/Tickets.txt");
            string temp = string.Empty;

            foreach (Ticket t in tickets)
            {
                temp = temp += t.ToString() + "\n";
            }

            System.IO.File.WriteAllText(path, temp);
        }

        public static void SaveTicket(Ticket t)
        {
            string path = HostingEnvironment.MapPath("~/App_Data/Tickets.txt");
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(t.ToString());
            }
        }

        public static List<Ticket> GetTickets()
        {
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "App_Data/Tickets.txt"))
                return new List<Ticket>();

            List<Ticket> list = new List<Ticket>();


            using (StreamReader reader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "App_Data/Tickets.txt"))
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

        public static Ticket FromString(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            var fields = str.Split('|');

            var ticket = new Ticket()
            {
                ID = int.Parse(fields[0]),
                CustomerID = int.Parse(fields[1]),
                SalesmanID = int.Parse(fields[2]),
                EventID = int.Parse(fields[3]),
                Type = (ETicketType)Enum.Parse(typeof(ETicketType), fields[4]),
                Price = double.Parse(fields[5]),
                Status = bool.Parse(fields[6]),
                IsDeleted = bool.Parse(fields[7])
            };

            //    .Append("|")
            //    .Append(Status.ToString())
            //    .Append("|")
            //    .Append(IsDeleted.ToString());

            return ticket;
        }

        public static string ListToString(List<Ticket> list)
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
