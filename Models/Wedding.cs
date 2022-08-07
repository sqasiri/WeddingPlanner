using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeddingPlanner.Models
{
    public class Wedding
    {
        [Key]
        public int WeddingId{get;set;}
        
        [Required(ErrorMessage = "Wedder One is required")]
        public string WedderOne{get;set;}
        [Required(ErrorMessage = "Wedder Two is required! are you marryign yourself??")]
        public string WedderTwo{get;set;}

        [Required(ErrorMessage = "Date is required")]
        public DateTime Date{get;set;}

        [Required(ErrorMessage = "Address is required")]
        public string Address{get;set;}
        public DateTime CreatedAt = DateTime.Now;
        public DateTime UpdatedAt = DateTime.Now;
        public int CreatorId{get;set;}
        public User Creator{get;set;}
        public List<AttendRelation> Attendees{get;set;}

    }
}