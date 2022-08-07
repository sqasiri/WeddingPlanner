using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeddingPlanner.Models
{
    public class AttendRelation
    {
        [Key]
        public int AttendRelationId{get;set;}
        public int UserId{get;set;}
        public User Attendee{get;set;}
        public int WeddingId{get;set;}
        public Wedding WeddingEvent{get;set;}
    }
}