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
    public class CommentData : ICommentData
    {
        public IEnumerable<Comment> GetAll(int eventID)
        {
            return GetComments().Where(x => x.EventID == eventID).ToList();
        }

        public IEnumerable<Comment> GetAll()
        {
            return GetComments();
        }

        public static List<Comment> GetComments()
        {
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "App_Data/Comments.txt"))
                return new List<Comment>();

            List<Comment> list = new List<Comment>();


            using (StreamReader reader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "App_Data/Comments.txt"))
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

        public static void SaveComment(Comment c)
        {
            string path = HostingEnvironment.MapPath("~/App_Data/Comments.txt");
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(c.ToString());
            }
        }

        public static Comment FromString(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            var fields = str.Split('|');

            var comment = new Comment()
            {
                ID = int.Parse(fields[0]),
                CustomerID = int.Parse(fields[1]),
                EventID = int.Parse(fields[2]),
                Rating = int.Parse(fields[3]),
                Text = fields[4],
                IsDeleted = bool.Parse(fields[5])
            };

            return comment;
        }

        public static string ListToString(List<Comment> list)
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
