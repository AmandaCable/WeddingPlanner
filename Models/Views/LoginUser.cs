using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeddingPlanner.Models 
{
    public class LoginUser
    {
        [Display(Name = "Email: ")]
        [Required(ErrorMessage = "Please enter your email.")]
        public string LogEmail { get; set; }
        
        [Display(Name = "Password: ")]
        [Required(ErrorMessage = "Everyone knows you must enter a password.")]
        [DataType(DataType.Password)]
        public string LogPassword { get; set; }
    }
}