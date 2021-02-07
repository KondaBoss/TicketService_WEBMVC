using System;
using System.Collections.Generic;
using System.Text;

namespace TicketService.Data.Models
{
    public class Administrator : User
    {
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb
                .Append(ID.ToString())
                .Append("|")
                .Append(FirstName ?? string.Empty)
                .Append("|")
                .Append(LastName ?? string.Empty)
                .Append("|")
                .Append(UserName ?? string.Empty)
                .Append("|")
                .Append(Password ?? string.Empty)
                .Append("|")
                .Append(Gender.ToString())
                .Append("|")
                .Append(BirthDate.ToString("yyyy-MM-dd") ?? string.Empty);

            return sb.ToString();
        }


        //public bool IsAdministrator(string userName)
        //{
        //    var administrators = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath(HardCodedAdministratorsPath));

        //    return administrators.Contains(userName);
        //}
    }
}
