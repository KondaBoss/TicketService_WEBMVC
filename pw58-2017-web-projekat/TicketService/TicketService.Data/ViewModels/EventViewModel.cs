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
    public class EventViewModel
    {
        public int ID { get; set; }

        public int SalesmanID { get; set; }

        public int VenueID { get; set; }

        public int Capacity { get; set; }

        public int NumberOfRemainingTickets { get; set; }

        [Required]
        [DisplayName("Event name")]
        [StringLength(30)]
        public string Name { get; set; }

        [DisplayName("Event type")]
        public EEventType EventType { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Event date")]
        public DateTime EventDate { get; set; }

        [Required]
        [DisplayName("Venue")]
        [StringLength(30)]
        public string VenueName { get; set; }

        [Required]
        [DisplayName("Address")]
        [StringLength(30)]
        public string VenueAddress { get; set; }

        [DisplayName("Event poster")]
        [DataType(DataType.ImageUrl)]
        public string Poster { get; set; }

        //[DataType(DataType.Currency)]
        [DisplayName("Ticket price")]
        public double TicketPrice { get; set; }

        public double Rating { get; set; }
        
        public bool Status { get; set; }

        public List<CommentViewModel> CommentList { get; set; }

        public bool IsDeleted { get; set; }
    }
}
