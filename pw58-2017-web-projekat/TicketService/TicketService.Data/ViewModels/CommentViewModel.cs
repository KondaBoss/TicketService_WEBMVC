using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketService.Data.ViewModels
{
    public class CommentViewModel
    {
        public int ID { get; set; }

        [Required]
        [DisplayName("User name")]
        [StringLength(30)]
        public string CustomerUsername { get; set; }

        [DisplayName("Comment")]
        public string Text { get; set; }

        [DisplayName("Event Rating")]
        public int Rating { get; set; }

        public bool IsDeleted { get; set; }
    }
}
