using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace WeddingPlanner.Models
{
    public class Rsvp
    {
        [Key]
        public int RsvpId { get; set; }

        public int RsvpUserId { get; set; }
        public User RsvpUser { get; set; }


        public int RsvpWeddingId { get; set; }
        public Wedding RsvpWedding { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

}