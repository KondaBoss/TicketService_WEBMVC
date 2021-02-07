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
    public class CustomerTypeData : ICustomerTypeData
    {
        public IEnumerable<CustomerType> GetAll()
        {
            return GetCustomerTypes();
        }

        public static List<CustomerType> GetCustomerTypes()
        {
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "App_Data/CustomerTypes.txt"))
                return new List<CustomerType>();

            List<CustomerType> list = new List<CustomerType>();


            using (StreamReader reader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "App_Data/CustomerTypes.txt"))
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

        public static CustomerType FromString(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            var fields = str.Split('|');

            var customerType = new CustomerType()
            {
                ID = int.Parse(fields[0]),
                Name = fields[1],
                NumberOfPoints = int.Parse(fields[2]),
                Discount = double.Parse(fields[3]),
                IsDeleted = bool.Parse(fields[4])
            };
            
            return customerType;
        }

        public static string ListToString(List<CustomerType> list)
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
