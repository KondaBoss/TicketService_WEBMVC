using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TicketService.Data.Models
{
    public class Event
    {
        public int ID { get; set; }

        public int SalesmanID { get; set; }

        public int Capacity { get; set; }

        //[DataType(DataType.Currency)]
        [DisplayName("Ticket price")]
        public double TicketPrice { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Event Date")]
        public DateTime EventDate { get; set; }

        public int VenueID { get; set; }

        [Required]
        [DisplayName("Event name")]
        [StringLength(30)]
        public string Name { get; set; }

        [DisplayName("Event poster")]
        [DataType(DataType.ImageUrl)]
        public string Poster { get; set; }

        public bool Status { get; set; }

        [DisplayName("Event type")]
        public EEventType EventType { get; set; }

        public bool IsDeleted { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            //var dejt1 = Convert.ToString(string.Format("{0:yyyy-MM-dd HH:mm tt}", EventDate));

            sb
                .Append(ID.ToString())
                .Append("|")
                .Append(SalesmanID.ToString())
                .Append("|")
                .Append(VenueID.ToString())
                .Append("|")
                .Append(Name ?? string.Empty)
                .Append("|")
                .Append(Poster ?? string.Empty)
                .Append("|")
                .Append(EventDate.ToString("dd.MM.yyyy, HH:mm") ?? string.Empty)
                .Append("|")
                .Append(EventType.ToString())
                .Append("|")
                .Append(TicketPrice.ToString())
                .Append("|")
                .Append(Capacity.ToString())
                .Append("|")
                .Append(Status.ToString())
                .Append("|")
                .Append(IsDeleted.ToString());

            return sb.ToString();
        }
    }
}
