using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeddingPlanner.Models
{
public class User
{
    [Key]
    public int UserId {get;set;}
    [Required]
    [MinLength(2, ErrorMessage ="First name has to be at least 2 characters")]
    public string FirstName {get;set;}
    [Required]
    [MinLength(2, ErrorMessage ="Last name has to be at least 2 characters")]
    public string LastName {get;set;}
    [EmailAddress(ErrorMessage ="Not a valid email")]
    [Required]
    public string Email {get;set;}
    
    public int Balance {get;set;}
    [DataType(DataType.Password)]
    [Required]
    [MinLength(8, ErrorMessage="Password must be 8 characters or longer!")]
    public string Password {get;set;}
    public DateTime CreatedAt {get;set;} = DateTime.Now;
    public DateTime UpdatedAt {get;set;} = DateTime.Now;
    // Will not be mapped to your users table!
    [NotMapped]
    [Compare("Password", ErrorMessage ="Doesn't match password")]
    [DataType(DataType.Password)]
    public string Confirm {get;set;}

    public List<Wedding> MyWeddings{get;set;}

    public List<AttendRelation> Weddings{get;set;}

} 

}

