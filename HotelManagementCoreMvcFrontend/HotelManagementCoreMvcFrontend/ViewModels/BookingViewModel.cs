using System.ComponentModel.DataAnnotations;

namespace HotelManagementCoreMvcFrontend.ViewModels
{
    public class BookingViewModel
    {
        public Guid Id { get; set; }


        [Required]
        public Guid UserId { get; set; }
        [Required]
        public Guid RoomId { get; set; }

        [Required (ErrorMessage ="Check-In date is required")]
        public DateTime? CheckInDate { get; set; }
        [Required(ErrorMessage ="Check-Out date is required")]
         public DateTime? CheckOutDate { get; set; }
        }
}
