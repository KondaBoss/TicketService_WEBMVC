using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TicketService.Data.Models;

namespace TicketService.Data.ViewModels
{
    public class CreateEventViewModel
    {
        public int ID { get; set; }

        [Required]
        [DisplayName("Event name")]
        [StringLength(30)]
        public string Name { get; set; }

        [DisplayName("Event type")]
        public EEventType EventType { get; set; }

        public int Capacity { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Event Date")]
        public DateTime EventDate { get; set; }

        [DataType(DataType.Currency)]
        [DisplayName("Ticket price")]
        public double TicketPrice { get; set; }

        [Required]
        [StringLength(50)]
        public string VenueName { get; set; }
    }
}
