using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TicketService.Data.Models
{
    public class Comment
    {
        public int ID { get; set; }

        public int CustomerID { get; set; }

        public int EventID { get; set; }

        [DisplayName("Event Rating")]
        public int Rating { get; set; }

        [DisplayName("Comment")]
        public string Text { get; set; }

        public bool IsDeleted { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb
                .Append(ID.ToString())
                .Append("|")
                .Append(CustomerID.ToString())
                .Append("|")
                .Append(EventID.ToString())
                .Append("|")
                .Append(Rating.ToString())
                .Append("|")
                .Append(Text ?? string.Empty)
                .Append("|")
                .Append(IsDeleted.ToString());

            return sb.ToString();
        }
    }
}
