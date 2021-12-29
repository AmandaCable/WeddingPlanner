using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace WeddingPlanner.Models
{
    public class Wedding
    {
        [Key]
        public int WeddingId {get; set;}

        [Display(Name ="Wedder One: ")]
        [Required(ErrorMessage ="Name is required.")]
        public string WedderOne {get; set;}

        [Display(Name ="Wedder Two: ")]
        [Required(ErrorMessage ="Name is required.")]
        public string WedderTwo {get; set;}

        [Display(Name ="Date: ")]
        [Required(ErrorMessage ="Wedding date is required.")]
        public DateTime Date {get; set;} 

        [Display(Name ="Wedding Address: ")]
        [Required(ErrorMessage ="Wedding address is required.")]
        public string Address {get; set;}

        public int CreatorId {get; set;} 
        public User Creator {get; set;} //So you can see what user made this wedding


        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public List<Rsvp> Rsvp {get; set;}
    }
}