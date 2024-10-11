using System.ComponentModel.DataAnnotations;

namespace HotelManagementCoreMvcFrontend.Models
{
    public class Feedback
    {
        public Guid Id { get; set; }


        [Required]
        public Guid BookingId { get; set; }

        public DateTime? CheckoutTime {  get; set; }
        public string? HotelName { get; set; }
        public int? RoomNo { get; set; }

        [Required(ErrorMessage ="FeedBack is required")]
        public string FeedbackText { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

    }
}
