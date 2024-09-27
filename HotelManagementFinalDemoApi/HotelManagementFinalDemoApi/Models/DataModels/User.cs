using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace HotelManagementFinalDemoApi.Models.DataModels
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

     
        [StringLength(15)]
       public string FirstName { get; set; } = null!;

       
        [StringLength(15)]
      public  string LastName { get; set; } = null!;

        
        [EmailAddress]
        [StringLength(100)]
        [RegularExpression(@"^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]{2,7}$", ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = null!;

        
        public string PasswordHash { get; set; } = null!;

      
        public int Role { get; set; }

        public bool IsEmailVerified { get; set; } = false;
        public string? ProfileImage { get; set; }
        public Guid? CreatedBy { get; set; }


        public DateTime? CreatedAt { get; set; }

        public Guid? UpdatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? Token { get; set; }
        public bool IsActive { get; set; } = true;


        // Navigation Properties
        public virtual ICollection<Booking> Bookings { get; set; }
        
        public virtual ICollection<Hotel>Hotels { get; set; }


        


    }
 
}
