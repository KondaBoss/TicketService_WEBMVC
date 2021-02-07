using System;
using System.Collections.Generic;
using System.Text;

namespace TicketService.Data.Models
{
    public class Salesman : User
    {
        public bool IsDeleted { get; set; }

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
                .Append(BirthDate.ToString("yyyy-MM-dd") ?? string.Empty)
                .Append("|")
                .Append(IsDeleted.ToString());

            return sb.ToString();
        }
    }
}
