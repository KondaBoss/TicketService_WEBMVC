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
    public class AdministratorData : IAdministratorData
    {
        public IEnumerable<Administrator> GetAll()
        {
            return GetAdministrators();
        }

        public static void SaveAdministrators(List<Administrator> administrators)
        {
            string path = HostingEnvironment.MapPath("~/App_Data/Administrators.txt");
            string temp = string.Empty;

            foreach (Administrator administrator in administrators)
            {
                temp = temp += administrator.ToString() + "\n";
            }

            System.IO.File.WriteAllText(path, temp);
        }

        public static List<Administrator> GetAdministrators()
        {
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "App_Data/Administrators.txt"))
                return new List<Administrator>();

            List<Administrator> list = new List<Administrator>();


            using (StreamReader reader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "App_Data/Administrators.txt"))
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

        public static Administrator FromString(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            var fields = str.Split('|');

            var administrator = new Administrator()
            {
                ID = int.Parse(fields[0]),
                FirstName = fields[1],
                LastName = fields[2],
                UserName = fields[3],
                Password = fields[4],
                Gender = bool.Parse(fields[5]),
                BirthDate = DateTime.Parse(fields[6]),
                Role = EUserRole.Administrator
            };

            return administrator;
        }

        public static string ListToString(List<Administrator> list)
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
