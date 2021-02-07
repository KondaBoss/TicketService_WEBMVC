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
    public class CustomerData : ICustomerData
    {
        public IEnumerable<Customer> GetAll()
        {
            return GetCustomers();
        }

        public IEnumerable<Customer> GetAllForSalesman(int eventID)
        {
            throw new NotImplementedException();
        }

        public static void SaveCustomers(List<Customer> customers)
        {
            string path = HostingEnvironment.MapPath("~/App_Data/Customers.txt");
            string temp = string.Empty;

            foreach (Customer customer in customers)
            {
                temp = temp += customer.ToString() + "\n";
            }

            System.IO.File.WriteAllText(path, temp);
        }

        public static void SaveCustomer(Customer customer)
        {
            string path = HostingEnvironment.MapPath("~/App_Data/Customers.txt");
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(customer.ToString());
            }
        }
        
        public static List<Customer> GetCustomers()
        {
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "App_Data/Customers.txt"))
                return new List<Customer>();

            List<Customer> list = new List<Customer>();


            using (StreamReader reader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "App_Data/Customers.txt"))
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

        public static Customer FromString(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            var fields = str.Split('|');

            var customer = new Customer()
            {
                ID = int.Parse(fields[0]),
                FirstName = fields[1],
                LastName = fields[2],
                UserName = fields[3],
                Password = fields[4],
                Gender = bool.Parse(fields[5]),
                BirthDate = DateTime.Parse(fields[6]),
                NumberOfCollectedPoints = double.Parse(fields[7]),
                CustomerType = int.Parse(fields[8]),
                NumberOfTicketsCancelled = int.Parse(fields[9]),
                IsSuspitious = bool.Parse(fields[10]),
                IsDeleted = bool.Parse(fields[11]),
                Role = EUserRole.Customer
            };
            
            return customer;
        }

        public static string ListToString(List<Customer> list)
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
