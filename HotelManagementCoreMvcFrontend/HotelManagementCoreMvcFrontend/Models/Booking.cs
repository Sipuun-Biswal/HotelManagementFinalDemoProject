using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HotelManagementCoreMvcFrontend.Models
{
    public class Booking
    {
       
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
   
        public Guid RoomId { get; set; }

        [Required]
        public DateTime CheckInDate { get; set; }

       
        public DateTime CheckOutDate { get; set; }


        [Required]
        public Status Status { get; set; }
        public bool? IsFeedbackGiven { get; set; } 
        public string? FullName { get; set; }
        public int? RoomNo { get; set; }

        public string? HotelName { get; set; }

        public Guid CreatedBy { get; set; }


        public DateTime CreatedDate { get; set; } = DateTime.Now;


        public Guid? UpdatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
    public enum Status {
        Confirm = 1,
        Pending = 2,
        cancel = 3
    }
    
}
