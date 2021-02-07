using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TicketService.Web.Models
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        [StringLength(15)]
        public string FirstName { get; set; }

        [Required]
        [Display(Name ="Last Name")]
        [StringLength(30)]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Birth Date")]
        public DateTime BirthDate { get; set; }


        public bool Gender { get; set; }

        [Required]
        [Display(Name = "User name")]
        [StringLength(30)]
        public string UserName { get; set; }

        [Required]
        [StringLength(30)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirm password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "User name")]
        [StringLength(30)]
        public string UserName { get; set; }

        [Required]
        [StringLength(30)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}