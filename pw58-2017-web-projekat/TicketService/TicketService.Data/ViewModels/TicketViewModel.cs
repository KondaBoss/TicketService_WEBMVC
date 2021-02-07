using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketService.Data.Models;

namespace TicketService.Data.ViewModels
{
    public class TicketViewModel
    {
        public int ID { get; set; }

        public int EventID { get; set; }

        public int CustomerID { get; set; }

        [Required]
        [DisplayName("Event name")]
        [StringLength(30)]
        public string EventName { get; set; }

        [DataType(DataType.Currency)]
        [DisplayName("Ticket price")]
        public double TicketPrice { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Event Date")]
        public DateTime EventDate { get; set; }

        [Required]
        [DisplayName("Customer Name")]
        [StringLength(45)]
        public string CustomerName { get; set; }

        [DisplayName("Type of ticket")]
        public ETicketType Type { get; set; }

        [DisplayName("Ticket status")]
        public bool Status { get; set; }

        public bool IsDeleted { get; set; }
    }
}
