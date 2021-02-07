using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace TicketService.Data.Models
{
    public class Customer : User
    {
        [DisplayName("Collected points")]
        public double NumberOfCollectedPoints { get; set; }

        public int CustomerType { get; set; }

        public int NumberOfTicketsCancelled { get; set; }

        public bool IsSuspitious { get; set; }

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
                .Append(NumberOfCollectedPoints.ToString())
                .Append("|")
                .Append(CustomerType.ToString())
                .Append("|")
                .Append(NumberOfTicketsCancelled.ToString())
                .Append("|")
                .Append(IsSuspitious.ToString())
                .Append("|")
                .Append(IsDeleted.ToString());

            return sb.ToString();
        }
    }
}
