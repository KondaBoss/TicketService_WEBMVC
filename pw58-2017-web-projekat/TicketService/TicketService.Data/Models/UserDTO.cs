using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketService.Data.Models
{
    public class UserDTO
    {
        public int ID { get; set; }

        [Required]
        [DisplayName("User name")]
        [StringLength(30)]
        public string UserName { get; set; }

        [Required]
        [DisplayName("First Name")]
        [StringLength(15)]
        public string FirstName { get; set; }

        [Required]
        [DisplayName("Last Name")]
        [StringLength(30)]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Birth Date")]
        public DateTime BirthDate { get; set; }
        
        public bool Gender { get; set; }

        public EUserRole Role { get; set; }

        [DisplayName("Collected points")]
        public double? NumberOfCollectedPoints { get; set; }

        public int CustomerType { get; set; }

        public int? NumberOfTicketsCancelled { get; set; }

        public bool IsSuspitious { get; set; }

        public bool IsDeleted { get; set; }
    }
}
