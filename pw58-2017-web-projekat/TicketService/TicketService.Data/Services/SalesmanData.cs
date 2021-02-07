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
    public class SalesmanData : ISalesmanData
    {
        public IEnumerable<Salesman> GetAll()
        {
            return GetSalesmen();
        }

        public static void SaveSalesmen(List<Salesman> salesmen)
        {
            string path = HostingEnvironment.MapPath("~/App_Data/Salesmen.txt");
            string temp = string.Empty;

            foreach (Salesman salesman in salesmen)
            {
                temp = temp += salesman.ToString() + "\n";
            }

            System.IO.File.WriteAllText(path, temp);
        }

        public static void SaveSalesman(Salesman salesman)
        {
            string path = HostingEnvironment.MapPath("~/App_Data/Salesmen.txt");
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(salesman.ToString());
            }
        }

        public static List<Salesman> GetSalesmen()
        {
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "App_Data/Salesmen.txt"))
                return new List<Salesman>();

            List<Salesman> list = new List<Salesman>();


            using (StreamReader reader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "App_Data/Salesmen.txt"))
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

        public static Salesman FromString(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            var fields = str.Split('|');

            var salesman = new Salesman()
            {
                ID = int.Parse(fields[0]),
                FirstName = fields[1],
                LastName = fields[2],
                UserName = fields[3],
                Password = fields[4],
                Gender = bool.Parse(fields[5]),
                BirthDate = DateTime.Parse(fields[6]),
                IsDeleted = bool.Parse(fields[7]),
                Role = EUserRole.Salesman
            };
            
            return salesman;
        }

        public static string ListToString(List<Salesman> list)
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
