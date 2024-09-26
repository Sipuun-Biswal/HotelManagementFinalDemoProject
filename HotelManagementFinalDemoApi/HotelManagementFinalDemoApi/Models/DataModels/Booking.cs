using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HotelManagementFinalDemoApi.Models.DataModels
{
    public class Booking
    {
        [Key]
        public Guid Id { get; set; }

      
       
        public Guid UserId { get; set; }

        [ForeignKey("Room")]
       
        public Guid RoomId { get; set; }

        
        public DateTime CheckInDate { get; set; }

       
        public DateTime CheckOutDate { get; set; }

        [ForeignKey("BookingStatus")]
        
        public int Status { get; set; }

       
        
        public Guid CreatedBy { get; set; }

        
        public DateTime CreatedDate { get; set; }= DateTime.Now;

        
        public Guid? UpdatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }



        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
       
     
        public virtual Room Room { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        
    }
}
