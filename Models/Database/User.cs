using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace WeddingPlanner.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Display(Name = "First Name: ")]
        [Required(ErrorMessage = "First Name is required.")]
        [RegularExpression("^[A-Za-z ]+$", ErrorMessage = "Name may only contain letters and spaces")]
        [MinLength(2, ErrorMessage = "First Name must be at least 2 characters.")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name: ")]
        [Required(ErrorMessage = "Last Name is required.")]
        [RegularExpression("^[A-Za-z ]+$", ErrorMessage = "Name may only contain letters and spaces")]
        [MinLength(2, ErrorMessage = "Last Name must be at least 2 characters.")]
        public string LastName { get; set; }

        [Display(Name = "Email: ")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be 8 characters or longer.")]
        public string Password { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        [NotMapped]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }


        public List<Rsvp> Rsvp {get; set;}
        public List<Wedding> WeddingsCreated {get; set;} //gives list of weddings that the one user created
    }
}