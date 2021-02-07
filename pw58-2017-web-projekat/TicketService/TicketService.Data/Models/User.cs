using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TicketService.Data.Interfaces;

namespace TicketService.Data.Models
{
    public class User : IUser
    {
        public int ID{ get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Birth Date")]
        public DateTime BirthDate{ get; set; }

        [Required]
        [DisplayName("First Name")]
        [StringLength(15)]
        public string FirstName{ get; set; }

        [Required]
        [DisplayName("Last Name")]
        [StringLength(30)]
        public string LastName{ get; set; }

        public bool Gender{ get; set; }

        [Required]
        [DisplayName("User name")]
        [StringLength(30)]
        public string UserName{ get; set; }

        [Required]
        [StringLength(30)]
        [DataType(DataType.Password)]
        public string Password{ get; set; }

        public EUserRole Role { get; set; }
    }
}
