using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HotelManagementFinalDemoApi.Models.DataModels
{
    public class Feedback
    {

        [Key]
        public Guid Id { get; set; }

        public Guid BookingId { get; set; }

        public string FeedbackText { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        // Navigation Properties
        [ForeignKey("BookingId")]
        public virtual Booking? Booking { get; set; }
        
    }
}
