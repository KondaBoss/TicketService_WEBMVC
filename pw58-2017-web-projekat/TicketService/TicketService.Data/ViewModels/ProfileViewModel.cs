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
    public class ProfileViewModel
    {
        public int ID { get; set; }

        [Required]
        [DisplayName("First Name")]
        [StringLength(15)]
        public string FirstName { get; set; }

        [Required]
        [DisplayName("Last Name")]
        [StringLength(30)]
        public string LastName { get; set; }

        public bool Gender { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Birth Date")]
        public DateTime BirthDate { get; set; }
        
        [Required]
        [DisplayName("User name")]
        [StringLength(30)]
        public string UserName { get; set; }

        [Required]
        [StringLength(30)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        [Required]
        [Display(Name = "Old password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The given password does not match the old one.")]
        public string OldPassword { get; set; }

        [Required]
        [Display(Name = "New password")]
        [StringLength(30)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [Display(Name = "Confirm password")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [DisplayName("Collected points")]
        public double NumberOfCollectedPoints { get; set; }

        public int CustomerType { get; set; }

        public EUserRole Role { get; set; }
    }
}
